using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // --- 영구 데이터 (계정 귀속) ---
    public List<int> ownedItemIDs; // 장비ID 저장
    public int equippedWeaponID = -1;
    public int equippedArmorID = -1;
    public int equippedAccessoryID1 = -1; // 첫 번째 액세서리 슬롯
    public int equippedAccessoryID2 = -1; // 두 번째 액세서리 슬롯

    // --- 세션 데이터 (한 판 임시 저장) ---
    public SessionData currentSessionData;

    public GameSaveData()
    {
        ownedItemIDs = new List<int>();
        currentSessionData = null;
    }
}

[System.Serializable]
public class SessionData
{
    public int level = 1;
    public long maxExp = 3;
    public long currentExp = 0;
    public int maxHp = 100;
    public int atk = 5;
    public int def = 5;
    public int agi = 3;
    public int luc = 3;
    public int unspentStatPoints = 0;
    public long currentGold = 0;
    public int currentBP = 10;

    public float playerPosX;
    public float playerPosY;
}
