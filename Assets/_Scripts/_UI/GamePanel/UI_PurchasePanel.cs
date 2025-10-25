using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_PurchasePanel : UI_Popup
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button backButton;
    [SerializeField] private UI_EquipmentPanel equipmentPanel;

    private ItemData currentItemData;
    private Action onPurchaseSuccess;
    private int targetSlotIndex = -1; // (-1: 미지정, 0: 슬롯1, 1: 슬롯2)

    private void Awake()
    {
        purchaseButton.onClick.AddListener(OnClick_Purchase);
        equipButton.onClick.AddListener(OnClick_Equip);
        unequipButton.onClick.AddListener(OnClick_Unequip);
        backButton.onClick.AddListener(OnClick_Back);
    }

    public void Init(ItemData itemData, Action onPurchaseSuccessCallback)
    {
        currentItemData = itemData;
        onPurchaseSuccess = onPurchaseSuccessCallback;
        itemNameText.text = currentItemData.itemName;

        UpdateButtons();
    }

    public void SetSlotIndex(int slotIndex = -1)
    {
        targetSlotIndex = slotIndex;
    }

    private void UpdateButtons()
    {
        GameSaveData saveData = DataManager.Instance.saveData;
        bool isOwned = saveData.ownedItemIDs.Contains(currentItemData.itemID);
        bool isEquiped = false; // 이 아이템이 "지금 내가 선택한 이 슬롯"에 장착되어 있는지 확인

        if (currentItemData.itemType == ItemType.Accessory)
        {
            if (targetSlotIndex == 0) // 1번 슬롯을 보고 있는 경우
                isEquiped = (saveData.equippedAccessoryID1 == currentItemData.itemID);
            else if (targetSlotIndex == 1) // 2번 슬롯을 보고 있는 경우
                isEquiped = (saveData.equippedAccessoryID2 == currentItemData.itemID);
        }
        else if (currentItemData.itemType == ItemType.Weapon) 
            isEquiped = (saveData.equippedWeaponID == currentItemData.itemID);
        else if (currentItemData.itemType == ItemType.Armor)
            isEquiped = (saveData.equippedArmorID == currentItemData.itemID);

        // 아이템을 소유하지 않은 경우 활성화.
        purchaseButton.interactable = !isOwned;
        // 소유했지만 장착하지 않았을 때 활성화.
        equipButton.interactable = isOwned && !isEquiped;
        // 소유하고 장착한 경우에만 활성화.
        unequipButton.interactable = isOwned && isEquiped;
    }

    private void OnClick_Purchase()
    {
        bool success = GameManager.Instance.PurchaseItem(currentItemData);

        if (success)
        {
            onPurchaseSuccess?.Invoke(); // 성공 시에만 UI 새로고침 콜백 실행
            UpdateButtons(); // 팝업 자신의 버튼 상태 갱신
        }
    }

    private void OnClick_Equip()
    {
        GameManager.Instance.EquipItem(currentItemData, targetSlotIndex);

        equipmentPanel.UpdateAllSlots();
        Debug.Log($"{currentItemData.itemName} 장착 완료!");
        onPurchaseSuccess?.Invoke(); // 장비창/상점창 새로고침 콜백 실행
        UpdateButtons();
    }

    private void OnClick_Unequip()
    {
        GameManager.Instance.UnequipItem(currentItemData, targetSlotIndex);

        equipmentPanel.UpdateAllSlots();
        Debug.Log($"{currentItemData.itemName} 해제 완료!");
        onPurchaseSuccess?.Invoke();
        UpdateButtons();  
    }

    private void OnClick_Back()
    {
        Hide();
    }
}
