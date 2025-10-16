using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> allItems;

    // 아이템 ID를 사용하여 데이터 베이스에서 해당하는 ItemData를 찾아 반환한다.
    public ItemData GetItemByID(int id)
    {
        return allItems.FirstOrDefault(item => item.itemID == id);
    }
}
