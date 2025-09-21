using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("InGame UI")]
    [SerializeField] private Slider encounterGaugeSlider;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text bpText;
    [SerializeField] private Slider expSlider;

    [Header("Battle UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private Slider monsterHPSlider;
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private TMP_Text battleLogText;

    [Header("Result UI")]
    [SerializeField] private GameObject resultUI;
    [SerializeField] private TMP_Text currentMoneyText;
    [SerializeField] private TMP_Text plusMoneyText;
    [SerializeField] private Slider resultExpSlider;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text plusExpText;
    [SerializeField] private TMP_Text ResultLogText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowBattleUI(PlayerData player, int current, int max /*MonsterData monster*/)
    {
        battleUI.SetActive(true);

        UpdatePlayerHP(player.currentHp, player.maxHp);
        UpdateMonsterHP(current, max);
    }

    public void HideBattleUI()
    {
        battleUI.SetActive(false);
    }

    public void ShowResultUI()
    {
        resultUI.SetActive(true); 
    }

    public void OnClick_CloseResultPanel()
    {
        resultUI.SetActive(false);

        // 보상이 끝났다고 BattleManager에 알림
        BattleManager.instance.ProceedAfterResult();
    }

    public void UpdatePlayerHP(int current, int max)
    {
        playerHPSlider.maxValue = max;
        playerHPSlider.value = current;
        playerHPText.text = $"{current} / {max}";
    }

    public void UpdateMonsterHP(int current, int max)
    {
        monsterHPSlider.maxValue = max;
        monsterHPSlider.value = current;
    }

    public void UpdateEncounterGauge(float currentValue, float maxValue)
    {
        if (encounterGaugeSlider == null) return;
        encounterGaugeSlider.maxValue = maxValue;
        encounterGaugeSlider.value = currentValue;
    }

    public void UpdateLevel(int level)
    {
        if (levelText == null) return;
        levelText.text = $"Lv. {level}";
    }

    public void UpdateMoney(long amount)
    {
        if (moneyText == null) return;
        // ToString("N0")는 숫자에 1,000단위 ,를 찍어준다.
        moneyText.text = amount.ToString("N0");
    }

    public void UpdateBP(int amount)
    {
        if (bpText == null) return;
        bpText.text = $"{amount}";
    }

    public void UpdateExp(long currentExp, long maxExp)
    {
        if (expSlider == null) return;
        expSlider.maxValue = maxExp;
        expSlider.value = currentExp;
    }
}
