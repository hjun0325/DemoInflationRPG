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

        //UIManager.instance.ShowBattleUI(playerData, monsterCurrentHP, monsterMaxHP);
        await UniTask.Delay(1000); // ���� ���� ����.

        // -- ���� ���� ���� --
        while (playerData.currentHp > 0 && monsterCurrentHP > 0)
        {
            // [�÷��̾� ��]
            await ExecutePlayerTurnAsync();
            if (monsterCurrentHP <= 0) break;

            // [���� ��]
            await ExecuteMonsterTurnAsync();
            if (playerData.currentHp <= 0) break;

            await UniTask.Delay(1000); // �� ���� ����.
        }

        // -- ���� ���� --
        bool playerWin = playerData.currentHp > 0;
        //UIManager.instance.ShowResultUI(playerWin);

        // TODO: �¸� �� ���� ���

        GameManager.instance.EndBattle(playerWin);
    }

    // �÷��̾� ��.
    private async UniTask ExecutePlayerTurnAsync()
    {
        Debug.Log("�÷��̾� �� ����");
        // TODO: AGI ��� ���� ���� Ȯ�� ���

        // [����] - ���� ���� ������ ���� ����.
        int damage = Mathf.Max(1, playerData.atk - monsterDEF);
        monsterCurrentHP -= damage;
        //UIManager.instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);
        Debug.Log($"�÷��̾ {damage} ���ظ� �������ϴ�!");
        await UniTask.Delay(500); // Ÿ�� ���� �ð�.

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
        //UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        Debug.Log($"���Ͱ� {damage} ���ظ� �������ϴ�!");
        await UniTask.Delay(500); // Ÿ�� ���� �ð�.
    }
}
