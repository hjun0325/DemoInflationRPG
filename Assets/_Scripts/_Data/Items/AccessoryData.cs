using UnityEngine;

[CreateAssetMenu(fileName = "Accessory", menuName = "Scriptable Objects/Items/Accessory")]
public class AccessoryData : ItemData
{
    public string statBonusName;
    public int addStatBonus;
    public int mulStatBonus;

    private void OnValidate()
    {
        itemType = ItemType.Accessory;
    }
}
