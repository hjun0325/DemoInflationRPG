using Unity.VisualScripting;
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

    // 경험치 추가 함수.
    public void AddExperience(long amount)
    {
        currentExp += amount;
        while (currentExp >= maxExp)
        {
            LevelUp();
        }
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
}
