using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // �ֹ߼� ������.
    public int level;

    public long currentExp;
    public long maxExp;

    public int currentHp;
    public int maxHp;

    private const long BASE_EXP = 3;
    private const float GROWTH_FACTOR = 1.5f;
    private const float MULTIPLIER = 0.5f;

    public int atk;
    public int def;
    public int agi;
    public int luc;

    public int unspentStatPoints;
    public long currentGold;

    // �� ������ ������ �� ���� ȣ���Ͽ� �ֹ߼� ������ �ʱ�ȭ.
    public void InitializeForNewRun()
    {
        level = 1;
        maxExp = 3;
        currentExp = 0;

        maxHp = 100;
        currentHp = maxHp;

        atk = 5;
        def = 5;
        agi = 3;
        luc = 3;

        unspentStatPoints = 0;
        currentGold = 0;
    }

    // ����� ���� �����͸� ���� (�̾��ϱ� ��)
    public void ApplySessionData(SessionData data)
    {
        level = data.level;
        maxExp = data.maxExp;
        currentExp = data.currentExp;

        maxHp = data.maxHp;
        currentHp = maxHp;

        atk = data.atk;
        def = data.def;
        agi = data.agi;
        luc = data.luc;

        unspentStatPoints = data.unspentStatPoints;
        currentGold = data.currentGold;
    }

    //  ü�� ȸ�� �Լ�.
    public void HealToFull()
    {
        currentHp = maxHp;
        UIManager.Instance.UpdatePlayerHP(currentHp, maxHp);
    }

    // ��� �߰� �Լ�.
    public void AddGold(long amount)
    {
        if (amount <= 0) return;
        currentGold += amount;
        UIManager.Instance.UpdateMoney(currentGold);
    }

    // ����ġ �߰� �Լ�.
    public void AddExperience(long amount)
    {
        currentExp += amount;
        while (currentExp >= maxExp)
        {
            LevelUp();
        }

        UIManager.Instance.UpdateExp(currentExp, maxExp);
        UIManager.Instance.UpdateLevel(level);
    }

    // ������ �Լ�.
    private void LevelUp()
    {
        level++;
        currentExp -= maxExp;

        // ���� ������ �ʿ��� ����ġ ����
        maxExp = CalculateMaxExpForLevel(level);

        unspentStatPoints += 4;
        
    }

    // Ư�� ������ �ʿ��� �� ����ġ�� ����ϴ� �Լ�.
    private long CalculateMaxExpForLevel(int targetLevel)
    {
        return (long)(BASE_EXP + Mathf.Pow(targetLevel, GROWTH_FACTOR) * MULTIPLIER);
    }

    // UIManager�� ȣ���� ���� �й� �Լ�.
    public void ApplyStatPoints(int hpPoints, int atkPoints, int defPoints, int agiPoints, int lucPoints)
    {
        int totalPointsToSpend = hpPoints + atkPoints + defPoints + agiPoints + lucPoints;
        if (totalPointsToSpend > unspentStatPoints) return;

        unspentStatPoints -= totalPointsToSpend;

        maxHp += hpPoints * 5;
        currentHp += hpPoints * 5;
        atk += atkPoints * 3;
        def += defPoints * 3;
        agi += agiPoints * 2;
        luc += lucPoints * 1;
    }
}
