using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentPanel : UI_Popup
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private UI_PurchasePanel purchasePanel;

    [Header("Weapon Slot UI")]
    [SerializeField] private Button weaponButton;
    [SerializeField] private Image weaponImage;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private TMP_Text addATKText;
    [SerializeField] private TMP_Text mulATKText;

    [Header("Armor Slot UI")]
    [SerializeField] private Button armorButton;
    [SerializeField] private Image armorImage;
    [SerializeField] private TMP_Text armorName;
    [SerializeField] private TMP_Text addDEFText;
    [SerializeField] private TMP_Text mulDEFText;

    [Header("Accessary Slot UI")]
    [SerializeField] private Button accessoryButton1;
    [SerializeField] private Image accessoryImage1;
    [SerializeField] private TMP_Text accessoryName1;
    [SerializeField] private TMP_Text addStatText1;
    [SerializeField] private TMP_Text mulStatText1;
    [SerializeField] private Button accessoryButton2;
    [SerializeField] private Image accessoryImage2;
    [SerializeField] private TMP_Text accessoryName2;
    [SerializeField] private TMP_Text addStatText2;
    [SerializeField] private TMP_Text mulDStatext2;

    private void Start()
    {
        // 액세서리 버튼 클릭 시 슬롯 인덱스(0, 1) 전달
        accessoryButton1.onClick.AddListener(() => OnEquippedSlotClicked(ItemType.Accessory, 0));
        accessoryButton2.onClick.AddListener(() => OnEquippedSlotClicked(ItemType.Accessory, 1));
    }

    public override void Show()
    {
        base.Show();
        UpdateAllSlots();
    }

    public void UpdateAllSlots()
    {
        GameSaveData saveData = DataManager.Instance.saveData;

        UpdateWeaponSlot(saveData.equippedWeaponID);
        UpdateArmorSlot(saveData.equippedArmorID);
        UpdateAccessorySlot1(saveData.equippedAccessoryID1); // 1번 슬롯 함수 호출
        UpdateAccessorySlot2(saveData.equippedAccessoryID2); // 2번 슬롯 함수 호출
    }

    private void UpdateWeaponSlot(int itemID)
    {
        Color color = weaponImage.color;

        // 장착된 무기가 없을 경우 (ID = -1)
        if (itemID == -1)
        {
            color.a = 0.0f;
            weaponImage.color = color;
            weaponImage.sprite = null;
            weaponName.text = "No Item";
            addATKText.text = "";
            mulATKText.text = "";
            return;
        }
        color.a = 1.0f;
        weaponImage.color = color;

        WeaponData weapon = itemDatabase.GetItemByID(itemID) as WeaponData;
        if (weapon == null) return;

        weaponImage.sprite = weapon.itemIcon;
        weaponName.text = weapon.itemName;
        addATKText.text = $"ATK +{weapon.addAtkBonus}";
        mulATKText.text = $"ATK x{weapon.mulAtkBonus}%";
    }

    private void UpdateArmorSlot(int itemID)
    {
        Color color = armorImage.color;

        // 장착된 방어구가 없을 경우 (ID = -1)
        if (itemID == -1)
        {
            color.a = 0.0f;
            armorImage.color = color;
            armorImage.sprite = null;
            armorName.text = "No Item";
            addDEFText.text = "";
            mulDEFText.text = "";
            return;
        }

        color.a = 1.0f;
        armorImage.color = color;

        ArmorData armor = itemDatabase.GetItemByID(itemID) as ArmorData;
        if (armor == null) return;

        armorImage.sprite = armor.itemIcon;
        armorName.text = armor.itemName;
        addDEFText.text = $"DEF +{armor.addDefBonus}";
        mulDEFText.text = $"DEF x{armor.mulDefBonus}%";
    }

    private void OnEquippedSlotClicked(ItemType type, int slotIndex)
    {
        int itemID = -1;
        switch (type)
        {
            case ItemType.Weapon: itemID = DataManager.Instance.saveData.equippedWeaponID; break;
            case ItemType.Armor: itemID = DataManager.Instance.saveData.equippedArmorID; break;
            case ItemType.Accessory:
                // 인덱스에 맞는 ID 가져오기
                itemID = (slotIndex == 0) ? DataManager.Instance.saveData.equippedAccessoryID1 : DataManager.Instance.saveData.equippedAccessoryID2;
                break;
        }

        purchasePanel.SetSlotIndex(slotIndex);

    }

    private void UpdateAccessorySlot1(int itemID)
    {
        Color color = accessoryImage1.color;

        // 장착된 액세서리가 없을 경우 (ID = -1)
        if (itemID == -1)
        {
            color.a = 0.0f;
            accessoryImage1.color = color;
            accessoryImage1.sprite = null;
            accessoryName1.text = "No Item";
            addStatText1.text = "";
            return;
        }

        color.a = 1.0f;
        accessoryImage1.color = color;

        AccessoryData accessory = itemDatabase.GetItemByID(itemID) as AccessoryData;
        if (accessory == null) return;

        accessoryImage1.sprite = accessory.itemIcon;
        accessoryName1.text = accessory.itemName;
        addStatText1.text = $"{accessory.statBonusName} +{accessory.addStatBonus}";
    }

    private void UpdateAccessorySlot2(int itemID)
    {
        Color color = accessoryImage2.color;

        // 장착된 액세서리가 없을 경우 (ID = -1)
        if (itemID == -1)
        {
            color.a = 0.0f;
            accessoryImage2.color = color;
            accessoryImage2.sprite = null;
            accessoryName2.text = "No Item";
            addStatText2.text = "";
            return;
        }

        color.a = 1.0f;
        accessoryImage2.color = color;

        AccessoryData accessory = itemDatabase.GetItemByID(itemID) as AccessoryData;
        if (accessory == null) return;

        accessoryImage2.sprite = accessory.itemIcon;
        accessoryName2.text = accessory.itemName;
        addStatText2.text = $"{accessory.statBonusName} +{accessory.addStatBonus}";
    }

    public void OnClick_Back()
    {
        Hide();
    }
}
