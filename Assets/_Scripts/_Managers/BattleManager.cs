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
    public static BattleManager Instance;

    private PlayerData playerData;
    private MonsterData currentMonsterData;

    [SerializeField] private RectTransform monsterAttackEffectTransform;
    [SerializeField] private RectTransform playerAttackEffectTransform;

    [SerializeField] private Transform monsterDamagePanel;
    [SerializeField] private Transform playerDamagePanel;

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
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartBattle(MonsterData specificMonster = null)
    {
        // UniTask 시작. Forget()은 결과값을 기다리지 않을 때 사용.
        BattleRoutineAsync(specificMonster).Forget();
    }

    // UIManager가 결과 창 닫기 신호를 보내면 호출될 함수
    public void ProceedAfterResult()
    {
        // 대기 중인 BattleRoutineAsync를 깨움
        resultCompletionSource?.TrySetResult(true);
    }

    private async UniTask BattleRoutineAsync(MonsterData specificMonster)
    {
        await UniTask.SwitchToMainThread();

        // -- 전투 준비 단계 --
        playerData = FindAnyObjectByType<PlayerData>();

        if (specificMonster != null)
        {
            // 보스전: 지정된 몬스터 데이터 사용
            currentMonsterData = specificMonster;
        }
        else
        {
            // 일반전: 현재 구역에서 랜덤 소환 (기존 로직)
            currentMonsterData = MapManager.Instance.GetRandomMonsterFromZone();
        }

        // [플레이어 최종 스탯 계산]
        playerFinalATK = playerData.TotalAtk;
        playerFinalDEF = playerData.TotalDef;
        playerFinalAGI = playerData.TotalAgi;
        playerFinalLUC = playerData.TotalLuc;

        // [몬스터 최종 스탯 계산]
        monsterFinalATK = currentMonsterData.atk;
        monsterFinalDEF = currentMonsterData.def;
        monsterFinalAGI = currentMonsterData.agi;
        monsterMaxHP = currentMonsterData.hp;
        monsterCurrentHP = monsterMaxHP;

        // 데미지 계산용 유효 스탯 계산
        playerEffectiveDEF = (long)(7 * Mathf.Sqrt(playerFinalDEF));
        monsterEffectiveATK = (long)(7 * Mathf.Sqrt(monsterFinalATK));

        UIManager.Instance.ShowBattleUI(playerData, currentMonsterData, monsterMaxHP);
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

            await UniTask.Delay(200, DelayType.UnscaledDeltaTime); // 턴 사이 간격.
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

            UIManager.Instance.HideBattleUI();
            UIManager.Instance.ShowResultUI();

            if (specificMonster != null)
            {
                GameManager.Instance.BossDefeated();
            }

            // UI 연출 종료 대기.
            await UIManager.Instance.PlayRewardAnimationAsync(startMoney, result.gainedGold, startExp, result.gainedExp, maxExp, startLevel);

            // 플레이어 터치 대기.
            resultCompletionSource = new UniTaskCompletionSource<bool>();
            await resultCompletionSource.Task;
        }
        // 패배 시
        else
        {
            UIManager.Instance.HideBattleUI();
        }

        GameManager.Instance.EndBattle(result);
    }

    // 플레이어 턴.
    private async UniTask ExecutePlayerTurnAsync()
    {
        await UniTask.SwitchToMainThread();

        while (true)
        {
            // 데미지 계산
            float critChance = ((playerFinalAGI * 0.05f) + (playerFinalLUC * 0.1f)) / 100f;
            bool isCritical = Random.Range(0f, 1f) < critChance;

            float reductionRate = 0.3f * (monsterFinalDEF / (float)(monsterFinalDEF + playerFinalATK));
            long damage = (long)(playerFinalATK * (1 - reductionRate));
            if (isCritical) damage = (long)(damage * 1.5f);
            damage = (long)Mathf.Max(1, damage);

            // 데미지 분산.
            float variance = Random.Range(0.9f, 1.1f);
            long finalDamage = (long)(damage * variance);

            monsterCurrentHP -= (int)finalDamage;
            UIManager.Instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);

            EffectManager.Instance.PlayEffect(
            "PlayerAttack",                 // DB에 등록한 이펙트 이름
            monsterAttackEffectTransform.position, // 몬스터 이미지의 현재 위치
            monsterAttackEffectTransform);         // 이펙트가 생성될 부모 캔버스
            SoundManager.Instance.PlaySFX("PlayerAttack1");

            string damageTextEffect = isCritical ? "CriticalDamageText" : "DamageText";
            EffectManager.Instance.ShowDamageText(damageTextEffect, (int)finalDamage, monsterDamagePanel);

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
        await UniTask.SwitchToMainThread();

        float reductionRate = 0.3f * (playerEffectiveDEF / (float)(playerEffectiveDEF + monsterEffectiveATK));
        long damage = (long)(monsterEffectiveATK * (1 - reductionRate));
        damage = (long)Mathf.Max(1, damage);

        float variance = Random.Range(0.9f, 1.1f);
        long finalDamage = (long)(damage * variance);

        playerData.currentHp -= (int)finalDamage;
        UIManager.Instance.UpdatePlayerHP(playerData.currentHp, playerData.TotalMaxHp);

        EffectManager.Instance.PlayEffect(
            "MonsterAttack",              // DB에 등록한 이펙트 이름
            playerAttackEffectTransform.position,                    // 플레이어 이미지의 현재 위치
            playerAttackEffectTransform);        // 이펙트가 생성될 부모 캔버스
        SoundManager.Instance.PlaySFX("MonsterAttack1");

        EffectManager.Instance.ShowDamageText("DamageText", (int)finalDamage, playerDamagePanel);
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 타격 연출 시간.
    }
}
