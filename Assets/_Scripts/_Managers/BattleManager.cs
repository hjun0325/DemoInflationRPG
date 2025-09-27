using UnityEngine;
using Cysharp.Threading.Tasks; // UniTask ����� ���� �ʿ�

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

    // �÷��̾� ���� ����.
    private int playerFinalATK, playerFinalDEF, playerFinalAGI, playerFinalLUC;

    // ���� ���� ����.
    private int monsterFinalATK, monsterFinalDEF, monsterFinalAGI;
    private int monsterMaxHP;
    private int monsterCurrentHP;

    // ������ ��꿡 ���� ��ȿ�� ����.
    private long playerEffectiveDEF;
    private long monsterEffectiveATK;

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

    // UIManager�� ��� â �ݱ� ��ȣ�� ������ ȣ��� �Լ�
    public void ProceedAfterResult()
    {
        // ��� ���� BattleRoutineAsync�� ����
        resultCompletionSource?.TrySetResult(true);
    }

    private async UniTask BattleRoutineAsync()
    {
        // -- ���� �غ� �ܰ� --
        playerData = FindAnyObjectByType<PlayerData>();
        // TODO: MapManager�κ��� ���� ���� ������ �޾ƿ� ���� ����
        currentMonsterData = MapManager.instance.GetRandomMonsterFromZone();

        // [�÷��̾� ���� ���� ���]
        // TODO: ��� ���ʽ� �ջ�
        playerFinalATK = playerData.atk;
        playerFinalDEF = playerData.def;
        playerFinalAGI = playerData.agi;
        playerFinalLUC = playerData.luc;

        // [���� ���� ���� ���]
        monsterFinalATK = currentMonsterData.atk;
        monsterFinalDEF = currentMonsterData.def;
        monsterFinalAGI = currentMonsterData.agi;
        monsterMaxHP = currentMonsterData.hp;
        monsterCurrentHP = monsterMaxHP;

        // ������ ���� ��ȿ ���� ���
        playerEffectiveDEF = (long)(7 * Mathf.Sqrt(playerFinalDEF));
        monsterEffectiveATK = (long)(7 * Mathf.Sqrt(monsterFinalATK));

        UIManager.instance.ShowBattleUI(playerData, currentMonsterData, monsterMaxHP);
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

        // �¸� �Ǻ� �� ü�� ȸ��.
        bool playerWin = playerData.currentHp > 0;
        BattleResult result = new BattleResult { playerWin = playerWin };

        // �¸� ��
        if (playerWin)
        {
            // ���� ���.
            result.gainedExp = currentMonsterData.dropExp;
            result.gainedGold = currentMonsterData.dropGold;

            // ���� �� ���� ����
            long startMoney = playerData.currentGold;
            long startExp = playerData.currentExp;
            long maxExp = playerData.maxExp;
            int startLevel = playerData.level;

            UIManager.instance.HideBattleUI();
            UIManager.instance.ShowResultUI();

            // UI ���� ���� ���.
            await UIManager.instance.PlayRewardAnimationAsync(startMoney, result.gainedGold, startExp, result.gainedExp, maxExp, startLevel);

            // �÷��̾� ��ġ ���.
            resultCompletionSource = new UniTaskCompletionSource<bool>();
            await resultCompletionSource.Task;
        }
        // �й� ��
        else
        {
            UIManager.instance.HideBattleUI();
        }

        GameManager.instance.EndBattle(result);
    }

    // �÷��̾� ��.
    private async UniTask ExecutePlayerTurnAsync()
    {
        Debug.Log("�÷��̾� �� ����");
        while (true)
        {
            // ������ ���
            float critChance = ((playerFinalAGI * 0.05f) + (playerFinalLUC * 0.1f)) / 100f;
            bool isCritical = Random.Range(0f, 1f) < critChance;

            float reductionRate = 0.3f * (monsterFinalDEF / (float)(monsterFinalDEF + playerFinalATK));
            long damage = (long)(playerFinalATK * (1 - reductionRate));
            if (isCritical) damage = (long)(damage * 1.5f);
            damage = (long)Mathf.Max(1, damage);

            monsterCurrentHP -= (int)damage;
            UIManager.instance.UpdateMonsterHP(monsterCurrentHP, monsterMaxHP);
            Debug.Log($"�÷��̾ {damage} ���ظ� �������ϴ�!");

            await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // Ÿ�� ���� �ð�.
            if (monsterCurrentHP <= 0) break;

            // ���� ���� ����.
            float agiAdvantage = (float)playerFinalAGI / (playerFinalAGI + monsterFinalAGI);
            float nextAttackChance = 0.7f * Mathf.Max(0, (agiAdvantage - 0.5f) * 2);
            if (Random.Range(0f, 1f) >= nextAttackChance) break;
        }
    }

    // ���� ��.
    private async UniTask ExecuteMonsterTurnAsync()
    {
        Debug.Log("���� �� ����");

        float reductionRate = 0.3f * (playerEffectiveDEF / (float)(playerEffectiveDEF + monsterEffectiveATK));
        long damage = (long)(monsterEffectiveATK * (1 - reductionRate));
        damage = (long)Mathf.Max(1, damage);

        playerData.currentHp -= (int)damage;
        UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        Debug.Log($"���Ͱ� {damage} ���ظ� �������ϴ�!");
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime); // Ÿ�� ���� �ð�.
    }
}
