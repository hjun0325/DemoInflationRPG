using UnityEngine;

public class PlayerData : MonoBehaviour
{
    // 휘발성 데이터.
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

    // 새 게임을 시작할 때 마다 호출하여 휘발성 데이터 초기화.
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

    // 저장된 세션 데이터를 적용 (이어하기 용)
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

    //  체력 회복 함수.
    public void HealToFull()
    {
        currentHp = maxHp;
        UIManager.Instance.UpdatePlayerHP(currentHp, maxHp);
    }

    // 골드 추가 함수.
    public void AddGold(long amount)
    {
        if (amount <= 0) return;
        currentGold += amount;
        UIManager.Instance.UpdateMoney(currentGold);
    }

    // 경험치 추가 함수.
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

    // 레벨업 함수.
    private void LevelUp()
    {
        level++;
        currentExp -= maxExp;

        // 다음 레벨의 필요한 경험치 재계산
        maxExp = CalculateMaxExpForLevel(level);

        unspentStatPoints += 4;
        
    }

    // 특정 레벨에 필요한 총 경험치를 계산하는 함수.
    private long CalculateMaxExpForLevel(int targetLevel)
    {
        return (long)(BASE_EXP + Mathf.Pow(targetLevel, GROWTH_FACTOR) * MULTIPLIER);
    }

    // UIManager가 호출할 스탯 분배 함수.
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
