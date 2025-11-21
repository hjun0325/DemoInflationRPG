using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UI_StorePanel : UI_Popup
{
    [SerializeField] private TMP_Text currentGoldText;
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

    private void OnEnable()
    {
        // 함수 구독
        PlayerData.OnPlayerDataUpdated += UpdateGoldText;

        // 활성화되는 즉시 현재 골드로 갱신
        UpdateGoldText();
    }

    private void OnDisable()
    {
        PlayerData.OnPlayerDataUpdated -= UpdateGoldText;
    }

    public override void Show()
    {
        base.Show();
        RefreshSlots();
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
        {
            currentGoldText.text = $"Money {GameManager.Instance.PlayerData.currentGold:N0}";
        }
        else
        {
            currentGoldText.text = "Money 0";
        }
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


