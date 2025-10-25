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
    [SerializeField] private TMP_Text bonusHPText;
    [SerializeField] private TMP_Text plusHPText;
    [SerializeField] private TMP_Text plusBonusHPText;

    [SerializeField] private TMP_Text currentATKText;
    [SerializeField] private TMP_Text bonusATKText;
    [SerializeField] private TMP_Text plusATKText;
    [SerializeField] private TMP_Text plusBonusATKText;

    [SerializeField] private TMP_Text currentDEFText;
    [SerializeField] private TMP_Text bonusDEFText;
    [SerializeField] private TMP_Text plusDEFText;
    [SerializeField] private TMP_Text plusBonusDEFText;

    [SerializeField] private TMP_Text currentAGIText;
    [SerializeField] private TMP_Text bonusAGIText;
    [SerializeField] private TMP_Text plusAGIText;
    [SerializeField] private TMP_Text plusBonusAGIText;

    [SerializeField] private TMP_Text currentLUCText;
    [SerializeField] private TMP_Text bonusLUCText;
    [SerializeField] private TMP_Text plusLUCText;
    [SerializeField] private TMP_Text plusBonusLUCText;

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
        long plusHpAmount = (long)HpPointsAdded * 5;
        long plusAtkAmount = (long)AtkPointsAdded * 3;
        long plusDefAmount = (long)DefPointsAdded * 3;
        long plusAgiAmount = (long)AgiPointsAdded * 2; // 기획서 기준 2로 가정
        long plusLucAmount = (long)LucPointsAdded * 1;

        // --- HP ---
        // 순수 스탯.
        currentHPText.text = playerDataRef.baseMaxHp.ToString();
        // 장비 보너스 스탯.
        bonusHPText.text = $"(+{playerDataRef.itemBonusHp})";
        bonusHPText.gameObject.SetActive(playerDataRef.itemBonusHp > 0);
        // 스탯 포인트로 추가될 스탯.
        plusHPText.text = plusHpAmount.ToString();
        plusHPText.gameObject.SetActive(HpPointsAdded > 0);
        plusBonusHPText.gameObject.SetActive(false);

        // --- ATK ---
        // 순수 스탯.
        currentATKText.text = playerDataRef.baseAtk.ToString();
        // 장비 보너스 스탯.
        bonusATKText.text = $"(+{playerDataRef.itemBonusAtk})";
        bonusATKText.gameObject.SetActive(playerDataRef.itemBonusAtk > 0);
        // 스탯 포인트로 추가될 스탯.
        plusATKText.text = plusAtkAmount.ToString();
        plusATKText.gameObject.SetActive(AtkPointsAdded > 0);
        if (AtkPointsAdded > 0)
        {
            int additionalBonus = (int)((plusAtkAmount * (playerDataRef.mulAtkBonus * 0.01f)) - plusAtkAmount);
            plusBonusATKText.text = $"(+{additionalBonus})";
            plusBonusATKText.gameObject.SetActive(additionalBonus > 0);
        }
        else
        {
            plusBonusATKText.gameObject.SetActive(false);
        }

        // --- DEF ---
        // 순수 스탯.
        currentDEFText.text = playerDataRef.baseDef.ToString();
        // 장비 보너스 스탯.
        bonusDEFText.text = $"(+{playerDataRef.itemBonusDef})";
        bonusDEFText.gameObject.SetActive(playerDataRef.itemBonusDef > 0);
        // 스탯 포인트로 추가될 스탯.
        plusDEFText.text = plusDefAmount.ToString();
        plusDEFText.gameObject.SetActive(DefPointsAdded > 0);
        if (DefPointsAdded > 0)
        {
            int additionalBonus = (int)((plusDefAmount * (playerDataRef.mulDefBonus * 0.01f)) - plusDefAmount);
            plusBonusDEFText.text = $"(+{additionalBonus})";
            plusBonusDEFText.gameObject.SetActive(additionalBonus > 0);
        }
        else
        {
            plusBonusDEFText.gameObject.SetActive(false);
        }

        // --- AGI ---
        // 순수 스탯.
        currentAGIText.text = playerDataRef.baseAgi.ToString();
        // 장비 보너스 스탯.
        bonusAGIText.text = $"(+{playerDataRef.itemBonusAgi})";
        bonusAGIText.gameObject.SetActive(playerDataRef.itemBonusAgi > 0);
        // 스탯 포인트로 추가될 스탯.
        plusAGIText.text = plusAgiAmount.ToString();
        plusAGIText.gameObject.SetActive(AgiPointsAdded > 0);
        plusBonusAGIText.gameObject.SetActive(false);

        // --- LUC ---
        // 순수 스탯.
        currentLUCText.text = playerDataRef.baseLuc.ToString();
        // 장비 보너스 스탯.
        bonusLUCText.text = $"(+{playerDataRef.itemBonusLuc})";
        bonusLUCText.gameObject.SetActive(playerDataRef.itemBonusLuc > 0);
        // 스탯 포인트로 추가될 스탯.
        plusLUCText.text = plusLucAmount.ToString();
        plusLUCText.gameObject.SetActive(LucPointsAdded > 0);
        plusBonusLUCText.gameObject.SetActive(false);

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
