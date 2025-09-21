using UnityEngine;
public enum ItemType { Weapon, Armor, Accessory }

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;

    [Header("Stat Bonus")]
    public int hpBonus;
    public int atkBonus;
    public int defBonus;
}
