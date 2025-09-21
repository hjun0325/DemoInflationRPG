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
        maxExp = 100;

        maxHp = 100;
        currentHp = maxHp;

        atk = 5;
        def = 5;
        agi = 5;
        luc = 5;

        unspentStatPoints = 0;
        currentGold = 0;
    }
}
