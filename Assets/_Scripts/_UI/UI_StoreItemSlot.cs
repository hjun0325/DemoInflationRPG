using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button slotButton;
    [SerializeField] private TMP_Text itemNumberText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text addStatText;
    [SerializeField] private TMP_Text mulStatText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text priceText;

    private ItemData itemData;
    private UI_StorePanel ownerPanel;

    public void Init(ItemData item, UI_StorePanel owner,int slotNumber)
    {
        itemData = item;
        ownerPanel = owner;

        itemIcon.sprite = itemData.itemIcon;
        itemNameText.text = itemData.itemName;
        priceText.text = itemData.price.ToString("N0");

        itemNumberText.text = slotNumber.ToString();

        // 아이템 타입에 따라 다른 스탯 텍스트를 표시.
        if (itemData is WeaponData weapon)
        {
            addStatText.text = $"ATK +{weapon.addAtkBonus}";
            mulStatText.text = $"ATK x{weapon.mulAtkBonus}";
        }
        else if (itemData is ArmorData armor)
        {
            addStatText.text = $"DEF +{armor.addDefBonus}";
            mulStatText.text = $"DEF x{armor.mulDefBonus}";
        }
        else if (itemData is AccessoryData accessory)
        {
            addStatText.text = $"{accessory.statBonusName} +{accessory.addStatBonus}";
        }

        // 버튼 클릭 시, 상위 패널의 함수를 호출하도록 연결
        slotButton.onClick.AddListener(OnSlotClicked);
        Refresh();
    }

    public void Refresh()
    {
        if (itemData == null) return;

        bool isOwned = DataManager.Instance.saveData.ownedItemIDs.Contains(itemData.itemID);
        countText.text = isOwned ? "x1" : "x0";
    }

    private void OnSlotClicked()
    {
        // 상위 패널에 "이 아이템이 클릭되었다"고 알림
        ownerPanel.OnClick_Item(itemData);
    }
}
