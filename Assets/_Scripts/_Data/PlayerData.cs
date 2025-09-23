using Unity.VisualScripting;
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
        currentExp = 0;
        maxExp = 3;

        maxHp = 100;
        currentHp = maxHp;

        atk = 5;
        def = 5;
        agi = 5;
        luc = 5;

        unspentStatPoints = 0;
        currentGold = 0;
    }

    // ����ġ �߰� �Լ�.
    public void AddExperience(long amount)
    {
        currentExp += amount;
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
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
}
