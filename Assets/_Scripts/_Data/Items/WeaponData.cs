using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Items/Weapon")]
public class WeaponData : ItemData
{
    public int addAtkBonus;
    public int mulAtkBonus;

    private void OnValidate()
    {
        itemType = ItemType.Weapon;
    }
}
