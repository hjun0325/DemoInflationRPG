using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZoneData", menuName = "Scriptable Objects/ZoneData")]
public class ZoneData : ScriptableObject
{
    public string ZoneName;
    public int recommendedLevel;

    // 이 구역에서 등장할 몬스터들의 목록
    public List<MonsterData> appearingMonsters;
}
