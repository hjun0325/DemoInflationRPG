using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask 사용을 위해 필요

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private PlayerData playerData;

    // 임시 몬스터 데이터 (실제로는 MonsterData와 레벨로 생성)
    private int monsterCurrentHP;
    private int monsterMaxHP = 10;
    private int monsterATK = 7;
    private int monsterDEF = 1;
    //private int monsterAGI = 5;

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

    private async UniTask BattleRoutineAsync()
    {
        // -- 전투 준비 단계 --
        playerData = FindAnyObjectByType<PlayerData>();
        // TODO: MapManager로부터 실제 몬스터 데이터 받아와 스탯 설정
        monsterCurrentHP = monsterMaxHP;

        UIManager.instance.ShowBattleUI(playerData, monsterCurrentHP, monsterMaxHP);
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
        bool playerWin = playerData.currentHp > 0;
        if (playerWin)
        {
            UIManager.instance.HideBattleUI();
            UIManager.instance.ShowResultUI();

            // UIManager가 신호를 줄 때까지 여기서 무한정 대기
            resultCompletionSource = new UniTaskCompletionSource<bool>();
            await resultCompletionSource.Task;
        }
        else
        {

        }

        GameManager.instance.EndBattle(playerWin);
    }

    // UIManager가 결과 창 닫기 신호를 보내면 호출될 함수
    public void ProceedAfterResult()
    {
        // 대기 중인 BattleRoutineAsync를 깨움
        resultCompletionSource?.TrySetResult(true);
    }

    // 플레이어 턴.
    private async UniTask ExecutePlayerTurnAsync()
    {
        Debug.Log("플레이어 턴 시작");
        // TODO: AGI 기반 연쇄 공격 확률 계산

        // [공격] - 추후 실제 데미지 공식 적용.
        int damage = Mathf.Max(1, playerData.atk - monsterDEF);
        monsterCurrentHP -= damage;
        UIManager.instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);
        Debug.Log($"플레이어가 {damage} 피해를 입혔습니다!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 타격 연출 시간.

        // [연쇄 공격 루프] - 추후 구현.
        /*while(Random.Range(0f,1f)< chainAttackChance)
        {

        }*/
    }

    // 몬스터 턴.
    private async UniTask ExecuteMonsterTurnAsync()
    {
        Debug.Log("몬스터 턴 시작");

        // [공격] - 추후 실제 데미지 공식 적용.
        int damage = Mathf.Max(1, monsterATK - playerData.def);
        playerData.currentHp -= damage;
        UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        Debug.Log($"몬스터가 {damage} 피해를 입혔습니다!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // 타격 연출 시간.
    }
}
