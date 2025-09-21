using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask ����� ���� �ʿ�

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    private PlayerData playerData;

    // �ӽ� ���� ������ (�����δ� MonsterData�� ������ ����)
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
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� ����.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle()
    {
        // UniTask�� �����Ѵ�. Forget()�� ������� ��ٸ��� ���� �� ���.
        BattleRoutineAsync().Forget();
    }

    private async UniTask BattleRoutineAsync()
    {
        // -- ���� �غ� �ܰ� --
        playerData = FindAnyObjectByType<PlayerData>();
        // TODO: MapManager�κ��� ���� ���� ������ �޾ƿ� ���� ����
        monsterCurrentHP = monsterMaxHP;

        UIManager.instance.ShowBattleUI(playerData, monsterCurrentHP, monsterMaxHP);
        await UniTask.Delay(1000, DelayType.UnscaledDeltaTime); // ���� ���� ����.

        // -- ���� ���� ���� --
        while (playerData.currentHp > 0 && monsterCurrentHP > 0)
        {
            // [�÷��̾� ��]
            await ExecutePlayerTurnAsync();
            if (monsterCurrentHP <= 0) break;

            // [���� ��]
            await ExecuteMonsterTurnAsync();
            if (playerData.currentHp <= 0) break;

            await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // �� ���� ����.
        }

        // -- ���� ���� --
        bool playerWin = playerData.currentHp > 0;
        if (playerWin)
        {
            UIManager.instance.HideBattleUI();
            UIManager.instance.ShowResultUI();

            // UIManager�� ��ȣ�� �� ������ ���⼭ ������ ���
            resultCompletionSource = new UniTaskCompletionSource<bool>();
            await resultCompletionSource.Task;
        }
        else
        {

        }

        GameManager.instance.EndBattle(playerWin);
    }

    // UIManager�� ��� â �ݱ� ��ȣ�� ������ ȣ��� �Լ�
    public void ProceedAfterResult()
    {
        // ��� ���� BattleRoutineAsync�� ����
        resultCompletionSource?.TrySetResult(true);
    }

    // �÷��̾� ��.
    private async UniTask ExecutePlayerTurnAsync()
    {
        Debug.Log("�÷��̾� �� ����");
        // TODO: AGI ��� ���� ���� Ȯ�� ���

        // [����] - ���� ���� ������ ���� ����.
        int damage = Mathf.Max(1, playerData.atk - monsterDEF);
        monsterCurrentHP -= damage;
        UIManager.instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);
        Debug.Log($"�÷��̾ {damage} ���ظ� �������ϴ�!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // Ÿ�� ���� �ð�.

        // [���� ���� ����] - ���� ����.
        /*while(Random.Range(0f,1f)< chainAttackChance)
        {

        }*/
    }

    // ���� ��.
    private async UniTask ExecuteMonsterTurnAsync()
    {
        Debug.Log("���� �� ����");

        // [����] - ���� ���� ������ ���� ����.
        int damage = Mathf.Max(1, monsterATK - playerData.def);
        playerData.currentHp -= damage;
        UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        Debug.Log($"���Ͱ� {damage} ���ظ� �������ϴ�!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // Ÿ�� ���� �ð�.
    }
}
