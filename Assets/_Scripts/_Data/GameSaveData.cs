using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    // 계정 전체에 영구적으로 저장될 데이터.
    public List<int> ownedItemIDs; // 장비ID 저장.

    // 한 판의 플레이 동안만 임시로 저장될 데이터. (이어하기 용)
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
    public int currentBP = 2;
}
