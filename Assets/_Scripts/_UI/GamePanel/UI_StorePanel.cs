using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class UI_StorePanel : UI_Popup
{
    [SerializeField] private ItemType storeType;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private UI_PurchasePanel purchasePanel;

    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform slotContainer;

    private List<UI_StoreItemSlot> createdSlots = new List<UI_StoreItemSlot>();

    private void Start()
    {
        // 게임 시작 시 슬롯들을 모두 생성
        InitializeSlots();
    }

    public override void Show()
    {
        base.Show();
        RefreshSlots();
    }

    private void InitializeSlots()
    {
        var itemsToShow =
            itemDatabase.AllItems.Where(item => item.itemType == storeType).ToList();

        int slotNumber = 1; // 슬롯 번호 카운터.
        foreach (var itemData in itemsToShow)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, slotContainer);
            UI_StoreItemSlot slot = slotGO.GetComponent<UI_StoreItemSlot>();
            slot.Init(itemData, this, slotNumber);
            slotNumber++;
            createdSlots.Add(slot);
        }
    }

    private void RefreshSlots()
    {
        foreach (var slot in createdSlots)
        {
            slot.Refresh();
        }
    }

    /*private void UpdatePanel()
    {
        // 아이템 데이터베이스에서 각 상점(storeType)에 맞는 아이템들만 가져온다.
        var itemsToShow = 
            itemDatabase.AllItems.Where(item => item.itemType == storeType).ToList();
        // DataManager에서 현재 플레이어가 소유한 아이템 ID 목록을 가져온다.
        var ownedItemIDs = DataManager.Instance.saveData.ownedItemIDs;

        for(int i = 0; i<itemSlots.Count; i++)
        {
            if (i < itemsToShow.Count)
            {
                ItemData currentItem = itemsToShow[i];
                StoreSlotUI currentSlot = itemSlots[i];
                Debug.Log(currentItem.itemIcon);
                // 슬롯 UI 채우기
                currentSlot.itemIcon.sprite = currentItem.itemIcon;
                currentSlot.itemNameText.text = currentItem.itemName;
                currentSlot.priceText.text = currentItem.price.ToString("N0");

                // 아이템 타입에 따라 다른 스탯 텍스트를 표시.
                if(currentItem is WeaponData weapon)
                {
                    currentSlot.addStatText.text = $"ATK +{weapon.addAtkBonus}";
                    currentSlot.mulStatText.text = $"ATK x{weapon.mulAtkBonus}";
                }
                else if (currentItem is ArmorData armor)
                {
                    currentSlot.addStatText.text = $"DEF +{armor.addDefBonus}";
                    currentSlot.mulStatText.text = $"DEF x{armor.mulDefBonus}";
                }
                else if (currentItem is AccessoryData accessory)
                {
                    
                }
            }
        }
    }*/

    public void OnClick_Item(ItemData itemData)
    {
        Debug.Log($"{itemData.itemName} 클릭됨!");
        purchasePanel.Show();
        purchasePanel.Init(itemData, () => { RefreshSlots(); });
    }

    public void OnClick_Back()
    {
        Hide();
    }
}


