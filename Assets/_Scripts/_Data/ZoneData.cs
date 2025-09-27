using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZoneData", menuName = "Scriptable Objects/ZoneData")]
public class ZoneData : ScriptableObject
{
    public string ZoneName;
    public int recommendedLevel;

    // �� �������� ������ ���͵��� ���
    public List<MonsterData> appearingMonsters;
}
