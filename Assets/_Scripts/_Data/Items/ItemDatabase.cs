using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> allItems;

    // ������ ID�� ����Ͽ� ������ ���̽����� �ش��ϴ� ItemData�� ã�� ��ȯ�Ѵ�.
    public ItemData GetItemByID(int id)
    {
        return allItems.FirstOrDefault(item => item.itemID == id);
    }
}
