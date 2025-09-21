using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    public int monsterID;
    public string monsterName;
    public Sprite monsterIcon;

    [Header("Stats")]
    public int hp;
    public int atk;
    public int def;
    public int agi;

    [Header("Rewards")]
    public int dropExp;
    public int dropGold;
}
