using TMPro;
using UnityEngine;

public class UI_StatusPanel : UI_Popup
{
    private PlayerData playerDataRef;

    // 임시 변수.
    private int unspentPoints;
    private int HpPointsAdded, AtkPointsAdded, DefPointsAdded, AgiPointsAdded, LucPointsAdded;

    // UI 참조 변수들.
    [Header("UI References")]
    [SerializeField] private TMP_Text currentHPText;
    [SerializeField] private TMP_Text plusHPText;
    [SerializeField] private TMP_Text currentATKText;
    [SerializeField] private TMP_Text plusATKText;
    [SerializeField] private TMP_Text currentDEFText;
    [SerializeField] private TMP_Text plusDEFText;
    [SerializeField] private TMP_Text currentAGIText;
    [SerializeField] private TMP_Text plusAGIText;
    [SerializeField] private TMP_Text currentLUCText;
    [SerializeField] private TMP_Text plusLUCText;
    [SerializeField] private TMP_Text statPointsText;

    public void Init(PlayerData playerData)
    {
        playerDataRef = playerData;
        ResetStats();
        UpdateUI();
    }

    // 임시 변수 초기화.
    private void ResetStats()
    {
        unspentPoints = playerDataRef.unspentStatPoints;
        HpPointsAdded = 0;
        AtkPointsAdded = 0;
        DefPointsAdded = 0;
        AgiPointsAdded = 0;
        LucPointsAdded = 0;
    }

    // 스탯 값을 갱신하는 함수.
    private void UpdateUI()
    {
        // --- HP ---
        currentHPText.text = playerDataRef.maxHp.ToString();
        plusHPText.text = $"(+{(long)HpPointsAdded * 5})";
        plusHPText.gameObject.SetActive(HpPointsAdded > 0);

        // --- ATK ---
        currentATKText.text = playerDataRef.atk.ToString();
        plusATKText.text = $"(+{(long)AtkPointsAdded * 3})";
        plusATKText.gameObject.SetActive(AtkPointsAdded > 0);

        // --- DEF ---
        currentDEFText.text = playerDataRef.def.ToString();
        plusDEFText.text = $"(+{(long)DefPointsAdded * 3})";
        plusDEFText.gameObject.SetActive(DefPointsAdded > 0);

        // --- AGI ---
        currentAGIText.text = playerDataRef.agi.ToString();
        plusAGIText.text = $"(+{(long)AgiPointsAdded * 2})";
        plusAGIText.gameObject.SetActive(AgiPointsAdded > 0);

        // --- LUC ---
        currentLUCText.text = playerDataRef.luc.ToString();
        plusLUCText.text = $"(+{(long)LucPointsAdded * 1})";
        plusLUCText.gameObject.SetActive(LucPointsAdded > 0);

        statPointsText.text = $"{unspentPoints}";
    }

    // --- 버튼 이벤트 ---

    public void OnClick_AddPoint(string statType)
    {
        if(unspentPoints <=0 ) return;

        unspentPoints--;
        switch(statType)
        {
            case "HP": HpPointsAdded++; break;
            case "ATK": AtkPointsAdded++; break;
            case "DEF": DefPointsAdded++; break;
            case "AGI": AgiPointsAdded++; break;
            case "LUC": LucPointsAdded++; break;
        }

        UpdateUI();
    }

    public void OnClick_AddHalf(string statType)
    {
        int pointsToAdd = (unspentPoints % 2 == 0) ? (unspentPoints / 2) : (unspentPoints / 2 + 1);
        if (pointsToAdd <= 0) return;

        unspentPoints -= pointsToAdd;
        switch(statType)
        {
            case "HP": HpPointsAdded += pointsToAdd; break;
            case "ATK": AtkPointsAdded += pointsToAdd; break;
            case "DEF": DefPointsAdded += pointsToAdd; break;
            case "AGI": AgiPointsAdded += pointsToAdd; break;
            case "LUC": LucPointsAdded += pointsToAdd; break;
        }

        UpdateUI();
    }

    public void OnClick_AddAll(string statType)
    {
        int pointsToAdd = unspentPoints;
        if (pointsToAdd <= 0) return;

        unspentPoints = 0;
        switch(statType)
        {
            case "HP": HpPointsAdded += pointsToAdd; break;
            case "ATK": AtkPointsAdded += pointsToAdd; break;
            case "DEF": DefPointsAdded += pointsToAdd; break;
            case "AGI": AgiPointsAdded += pointsToAdd; break;
            case "LUC": LucPointsAdded += pointsToAdd; break;
        }

        UpdateUI();
    }

    public void OnClick_Decision()
    {
        playerDataRef?.ApplyStatPoints(
            HpPointsAdded, AtkPointsAdded, DefPointsAdded, AgiPointsAdded, LucPointsAdded);
        DataManager.Instance.UpdateSaveData();
        ResetStats();
        UpdateUI();
    }

    public void OnClick_Cancel()
    {
        ResetStats();
        UpdateUI();
    }

    public void OnClick_Back()
    {
        Hide();        
    }
}
