using UnityEngine;
public enum ItemType { Weapon, Armor, Accessory }

public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public int price;
}
