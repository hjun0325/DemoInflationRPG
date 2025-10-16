using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Scriptable Objects/Items/Armor")]
public class ArmorData : ItemData
{
    public int addDefBonus;
    public int mulDefBonus;

    private void OnValidate()
    {
        itemType = ItemType.Armor;
    }
}
