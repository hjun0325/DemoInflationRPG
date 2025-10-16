using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class StoreSlotUI
{
    public Image itemIcon;
    public Button slotButton;
    public TMP_Text itemNameText;
    public TMP_Text addStatText;
    public TMP_Text mulStatText;
    public TMP_Text countText;
    public TMP_Text priceText;
}

public class UI_StorePanel : UI_Popup
{
    [SerializeField] private ItemType storeType;
    [SerializeField] private ItemDatabase itemDatabase;

    [SerializeField] private List<StoreSlotUI> itemSlots;

    public override void Show()
    {
        base.Show();
        UpdatePanel();
    }

    private void UpdatePanel()
    {
        
    }

    public void OnClick_Back()
    {
        Hide();
    }

    public void OnClick_Item(ItemData itemData)
    {
        Debug.Log($"{itemData.itemName} 클릭됨!");
        // TODO: 구매 또는 장착 로직
    }
}


