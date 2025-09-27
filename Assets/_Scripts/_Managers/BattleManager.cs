using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask 사용을 위해 필요

public struct BattleResult
{
    public bool playerWin;
    public long gainedExp;
    public long gainedGold;
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private PlayerData playerData;
    private MonsterData currentMonsterData;

    // 플레이어 최종 스탯.
    private int playerFinalATK, playerFinalDEF, playerFinalAGI, playerFinalLUC;

    // 몬스터 최종 스탯.
    private int monsterFinalATK, monsterFinalDEF, monsterFinalAGI;
    private int monsterMaxHP;
    private int monsterCurrentHP;

    // 데미지 계산에 사용될 유효한 스탯.
    private long playerEffectiveDEF;
    private long monsterEffectiveATK;

    private UniTaskCompletionSource<bool> resultCompletionSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle()
    {
        // UniTask를 시작한다. Forget()은 결과값을 기다리지 않을 때 사용.
        BattleRoutineAsync().Forget();
    }

    // UIManager가 결과 창 닫기 신호를 보내면 호출될 함수
    public void ProceedAfterResult()
    {
        // 대기 중인 BattleRoutineAsync를 깨움
        resultCompletionSource?.TrySetResult(true);
    }

    private async UniTask BattleRoutineAsync()
    {
        // -- 전투 준비 단계 --
        playerData = FindAnyObjectByType<PlayerData>();
        // TODO: MapManager로부터 실제 몬스터 데이터 받아와 스탯 설정
        currentMonsterData = MapManager.instance.GetRandomMonsterFromZone();

        // [플레이어 최종 스탯 계산]
        // TODO: 장비 보너스 합산
        playerFinalATK = playerData.atk;
        playerFinalDEF = playerData.def;
        playerFinalAGI = playerData.agi;
        playerFinalLUC = playerData.luc;

        // [몬스터 최종 스탯 계산]
        monsterFinalATK = currentMonsterData.atk;
        monsterFinalDEF = currentMonsterData.def;
        monsterFinalAGI = currentMonsterData.agi;
        monsterMaxHP = currentMonsterData.hp;
        monsterCurrentHP = monsterMaxHP;

        // 데미지 계산용 유효 스탯 계산
        playerEffectiveDEF = (long)(7 * Mathf.Sqrt(playerFinalDEF));
        monsterEffectiveATK = (long)(7 * Mathf.Sqrt(monsterFinalATK));

        UIManager.instance.ShowBattleUI(playerData, currentMonsterData, monsterMaxHP);
        await UniTask.Delay(1000, DelayType.UnscaledDeltaTime); // 전투 시작 연출.

        // -- 전투 루프 시작 --
        while (playerData.currentHp > 0 && monsterCurrentHP > 0)
        {
            // [플레이어 턴]
            await ExecutePlayerTurnAsync();
            if (monsterCurrentHP <= 0) break;

            // [몬스터 턴]
            await ExecuteMonsterTurnAsync();
            if (playerData.currentHp <= 0) break;

            await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 턴 사이 간격.
        }

        // -- 전투 종료 --

        // 승리 판별 후 체력 회복.
        bool playerWin = playerData.currentHp > 0;
        BattleResult result = new BattleResult { playerWin = playerWin };

        // 승리 시
        if (playerWin)
        {
            // 보상 계산.
            result.gainedExp = currentMonsterData.dropExp;
            result.gainedGold = currentMonsterData.dropGold;

            // 연출 전 상태 저장
            long startMoney = playerData.currentGold;
            long startExp = playerData.currentExp;
            long maxExp = playerData.maxExp;
            int startLevel = playerData.level;

            UIManager.instance.HideBattleUI();
            UIManager.instance.ShowResultUI();

            // UI 연출 종료 대기.
            await UIManager.instance.PlayRewardAnimationAsync(startMoney, result.gainedGold, startExp, result.gainedExp, maxExp, startLevel);

            // 플레이어 터치 대기.
            resultCompletionSource = new UniTaskCompletionSource<bool>();
            await resultCompletionSource.Task;
        }
        // 패배 시
        else
        {
            UIManager.instance.HideBattleUI();
        }

        GameManager.instance.EndBattle(result);
    }

    // 플레이어 턴.
    private async UniTask ExecutePlayerTurnAsync()
    {
        Debug.Log("플레이어 턴 시작");
        while (true)
        {
            // 데미지 계산
            float critChance = ((playerFinalAGI * 0.05f) + (playerFinalLUC * 0.1f)) / 100f;
            bool isCritical = Random.Range(0f, 1f) < critChance;

            float reductionRate = 0.3f * (monsterFinalDEF / (float)(monsterFinalDEF + playerFinalATK));
            long damage = (long)(playerFinalATK * (1 - reductionRate));
            if (isCritical) damage = (long)(damage * 1.5f);
            damage = (long)Mathf.Max(1, damage);

            monsterCurrentHP -= (int)damage;
            UIManager.instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);
            Debug.Log($"플레이어가 {damage} 피해를 입혔습니다!");

            await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 타격 연출 시간.
            if (monsterCurrentHP <= 0) break;

            // 연쇄 공격 판정.
            float agiAdvantage = (float)playerFinalAGI / (playerFinalAGI + monsterFinalAGI);
            float nextAttackChance = 0.7f * Mathf.Max(0, (agiAdvantage - 0.5f) * 2);
            if (Random.Range(0f, 1f) >= nextAttackChance) break;
        }
    }

    // 몬스터 턴.
    private async UniTask ExecuteMonsterTurnAsync()
    {
        Debug.Log("몬스터 턴 시작");

        float reductionRate = 0.3f * (playerEffectiveDEF / (float)(playerEffectiveDEF + monsterEffectiveATK));
        long damage = (long)(monsterEffectiveATK * (1 - reductionRate));
        damage = (long)Mathf.Max(1, damage);

        playerData.currentHp -= (int)damage;
        UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        Debug.Log($"몬스터가 {damage} 피해를 입혔습니다!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 타격 연출 시간.
    }
}
