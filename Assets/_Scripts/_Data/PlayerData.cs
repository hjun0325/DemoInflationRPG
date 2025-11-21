using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerData : MonoBehaviour
{
    public static event Action OnPlayerDataUpdated;

    [SerializeField] private ItemDatabase itemDatabase;

    // --- 기본 스탯 (스탯 포인트로만 증가) ---
    public int baseMaxHp { get; private set; }
    public int baseAtk { get; private set; }
    public int baseDef { get; private set; }
    public int baseAgi { get; private set; }
    public int baseLuc { get; private set; }

    // --- 장비 보너스 스탯 ---
    public int itemBonusHp { get; private set; }
    public int itemBonusAtk { get; private set; }
    public int itemBonusDef { get; private set; }
    public int itemBonusAgi { get; private set; }
    public int itemBonusLuc { get; private set; }
    public float mulAtkBonus { get; private set; }
    public float mulDefBonus { get; private set; }

    // --- 최종 스탯 ---
    public int TotalMaxHp => baseMaxHp + itemBonusHp;
    public int TotalAtk => baseAtk + itemBonusAtk;
    public int TotalDef => baseDef + itemBonusDef;
    public int TotalAgi => baseAgi + itemBonusAgi;
    public int TotalLuc => baseLuc + itemBonusLuc;

    public int level;
    public int currentHp;
    public long maxExp;
    public long currentExp;
    public int unspentStatPoints;
    public long currentGold;

    private const long BASE_EXP = 3;
    private const float GROWTH_FACTOR = 1.5f;
    private const float MULTIPLIER = 0.5f;

    // 새 게임을 시작할 때 마다 호출.
    public void InitializeForNewRun()
    {
        // 휘발성 데이터 초기화.
        baseMaxHp = 100; 
        baseAtk = 5;     
        baseDef = 5;     
        baseAgi = 3;     
        baseLuc = 3;     
        
        level = 1;
        currentExp = 0;
        maxExp = 3;
        unspentStatPoints = 0;
        currentGold = 0;

        RecalculateStats();

        currentHp = TotalMaxHp;
    }

    // 저장된 세션 데이터를 적용 (이어하기 용)
    public void ApplySessionData(SessionData data)
    {
        level = data.level;
        maxExp = data.maxExp;
        currentExp = data.currentExp;
        unspentStatPoints = data.unspentStatPoints;
        currentGold = data.currentGold;

        baseMaxHp = data.maxHp;
        baseAtk = data.atk;
        baseDef = data.def;
        baseAgi = data.agi;
        baseLuc = data.luc;

        // 스탯은 새로 계산
        RecalculateStats();

        currentHp = TotalMaxHp;

        transform.position = new Vector3(data.playerPosX, data.playerPosY, 0);
    }

    // 스탯 재계산.
    public void RecalculateStats()
    {
        itemBonusHp = 0;
        itemBonusAtk = 0;
        itemBonusDef = 0;
        itemBonusAgi = 0;
        itemBonusLuc = 0;

        // 장착 장비.
        var saveData = DataManager.Instance.saveData;
        List<int> equippedIDs = new List<int>
        {
            saveData.equippedWeaponID,
            saveData.equippedArmorID,
            saveData.equippedAccessoryID1,
            saveData.equippedAccessoryID2
        };

        // 보너스 스탯 추가
        foreach (int itemID in equippedIDs)
        {
            if (itemID == -1) continue;

            ItemData item = itemDatabase.GetItemByID(itemID);
            if (item == null) continue;

            if(item is WeaponData weapon)
            {
                itemBonusAtk = (int)((weapon.addAtkBonus + baseAtk) * (weapon.mulAtkBonus / 100.0f) - baseAtk);
                mulAtkBonus = weapon.mulAtkBonus;
            }
            else if(item is ArmorData armor)
            {
                itemBonusDef = (int)((armor.addDefBonus + baseDef) * (armor.mulDefBonus / 100.0f) - baseDef);
                mulDefBonus = armor.mulDefBonus;
            }
            else if (item is AccessoryData accessory)
            {
                switch (accessory.statBonusName)
                {
                    case "ATK": itemBonusAtk += accessory.addStatBonus; break;
                    case "DEF": itemBonusDef += accessory.addStatBonus; break;
                    case "HP": itemBonusHp += accessory.addStatBonus; break;
                    case "AGI": itemBonusAgi += accessory.addStatBonus; break;
                    case "LUC": itemBonusLuc += accessory.addStatBonus; break;
                }
            }
        }

        if (currentHp > TotalMaxHp) currentHp = TotalMaxHp;

        Debug.Log($"스탯 재계산 완료: 최종 ATK={TotalAtk}, 최종 DEF={TotalDef}");

        OnPlayerDataUpdated?.Invoke();
    }

    // UIManager가 호출할 스탯 분배 함수.
    public void ApplyStatPoints(int hpPoints, int atkPoints, int defPoints, int agiPoints, int lucPoints)
    {
        int totalPointsToSpend = hpPoints + atkPoints + defPoints + agiPoints + lucPoints;
        if (totalPointsToSpend > unspentStatPoints) return;

        unspentStatPoints -= totalPointsToSpend;

        baseMaxHp += hpPoints * 5;
        currentHp += hpPoints * 5;
        baseAtk += atkPoints * 3;
        baseDef += defPoints * 3;
        baseAgi += agiPoints * 2;
        baseLuc += lucPoints * 1;

        RecalculateStats();

        OnPlayerDataUpdated?.Invoke();
    }

    //  체력 회복 함수.
    public void HealToFull()
    {
        currentHp = TotalMaxHp;
        UIManager.Instance.UpdatePlayerHP(currentHp, TotalMaxHp);
        //OnPlayerDataUpdated?.Invoke();
    }

    // 골드 추가 함수.
    public void AddGold(long amount)
    {
        currentGold += amount;
        OnPlayerDataUpdated?.Invoke();
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
        OnPlayerDataUpdated?.Invoke();

    }

    // 특정 레벨에 필요한 총 경험치를 계산하는 함수.
    private long CalculateMaxExpForLevel(int targetLevel)
    {
        return (long)(BASE_EXP + Mathf.Pow(targetLevel, GROWTH_FACTOR) * MULTIPLIER);
    }
}
