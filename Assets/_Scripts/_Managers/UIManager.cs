using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public GameObject JoystickUI;

    [Header("InGame UI")]
    [SerializeField] private Slider encounterGaugeSlider;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text bpText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Button MenuButton;

    [Header("Battle UI")]
    [SerializeField] private GameObject battleUI;
    [SerializeField] private Slider monsterHPSlider;
    [SerializeField] private Image monsterImage;
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private TMP_Text battleLogText;

    [Header("Result UI")]
    [SerializeField] private GameObject resultUI;
    [SerializeField] private Button closeResultButton;
    [SerializeField] private TMP_Text currentMoneyText;
    [SerializeField] private TMP_Text plusMoneyText;
    [SerializeField] private Slider resultExpSlider;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text plusExpText;
    [SerializeField] private TMP_Text ResultLogText;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private UI_StatusPanel statusPanel;
    [SerializeField] private UI_EquipmentPanel equipmentPanel;
    [SerializeField] private UI_StorePanel weaponStorePanel;
    [SerializeField] private UI_StorePanel armorStorePanel;
    [SerializeField] private UI_StorePanel accessaryStorePanel;

    [Header("Gameover UI")]
    [SerializeField] private GameObject gameoverUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        /*MenuButton?.onClick.AddListener(OnClick_ShowMenuPanel);
        closeResultButton?.onClick.AddListener(OnClick_CloseResultPanel);*/
    }

    // ���� ����� �񵿱� �Լ�.
    public async UniTask PlayRewardAnimationAsync(long startMoney, long gainedMoney, long startExp, long gainedExp, long maxExp, long currentLevel)
    {
        // �� ���� Task�� ���� �����ϰ� ����.
        var goldTask = PlayGoldAnimationAsync(startMoney, gainedMoney);
        var expTask = PlayExpAnimationAsync(startExp, gainedExp, maxExp, currentLevel);

        // UniTask.WhenAll�� ���� �� Task�� ��� ���� ������ ��ٸ�
        await UniTask.WhenAll(goldTask, expTask);

        Debug.Log("��� ���� ���� �Ϸ�!");
        closeResultButton.interactable = true;
    }

    // ��� ���� ����� �񵿱� �Լ�.
    private async UniTask PlayGoldAnimationAsync(long startMoney, long gainedMoney)
    {
        float goldAnimDuration = 2.0f; // ��� ���� �ð�
        float timer = 0f;
        plusMoneyText.text = $"+ {gainedMoney:N0}";

        while (timer < goldAnimDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / goldAnimDuration;
            long currentGainedMoney = (long)Mathf.Lerp(0, gainedMoney, progress);
            long remainingMoney = gainedMoney - currentGainedMoney;

            currentMoneyText.text = (startMoney + currentGainedMoney).ToString("N0");
            plusMoneyText.text = $"+ {remainingMoney:N0}";
            await UniTask.Yield();
        }
        // ������ ����
        currentMoneyText.text = (startMoney + gainedMoney).ToString("N0");
        plusMoneyText.text = "+0";
    }

    // ����ġ ���� ����� �񵿱� �Լ�.
    private async UniTask PlayExpAnimationAsync(long startExp, long gainedExp, long maxExp, long currentLevel)
    {
        long remainingExp = gainedExp;
        long currentDisplayExp = startExp;
        long maxDisplayExp = maxExp;
        long currentDisplayLevel = currentLevel;
        float tickSpeed = 0.01f;

        resultExpSlider.maxValue = maxDisplayExp;
        resultExpSlider.value = currentDisplayExp;

        while (remainingExp > 0)
        {
            long expToAdd = (long)(maxDisplayExp * 0.1f);
            if (expToAdd == 0) expToAdd = 1;
            expToAdd = (long)Mathf.Min(expToAdd, remainingExp);

            currentDisplayExp += expToAdd;
            remainingExp -= expToAdd;

            // UI ������Ʈ.
            resultExpSlider.value = currentDisplayExp;
            expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
            plusExpText.text = $"+ {remainingExp:N0}";
            
            if (currentDisplayExp >= maxDisplayExp)
            {
                currentDisplayLevel++;
                currentDisplayExp -= maxDisplayExp;
                maxDisplayExp = (long)(3 + Mathf.Pow(currentDisplayLevel, 1.5f) * 0.5f);
                resultExpSlider.value = 0; // �� �ʱ�ȭ.
                resultExpSlider.maxValue = maxDisplayExp;
                await UniTask.Delay(100, DelayType.UnscaledDeltaTime);
            }
            await UniTask.Delay((int)(tickSpeed * 500), DelayType.UnscaledDeltaTime);
        }
        resultExpSlider.value = currentDisplayExp; // ������ ����
        expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
    }

    public void ShowBattleUI(PlayerData player, MonsterData monster, int monsterMaxHP)
    {
        battleUI.SetActive(true);

        monsterImage.sprite = monster.monsterIcon;
        UpdatePlayerHP(player.currentHp, player.maxHp);
        UpdateMonsterHP(monsterMaxHP, monsterMaxHP);
    }

    public void HideBattleUI()
    {
        battleUI.SetActive(false);
    }

    public void ShowResultUI()
    {
        resultUI.SetActive(true);
        closeResultButton.interactable = false;
    }

    public void ShowGameoverUI()
    {
        gameoverUI.SetActive(true);
    }

    public void OnClick_LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void OnClick_ShowMenuPanel()
    {
        menuUI.SetActive(true);
        menuPanel.SetActive(true);
        GameManager.Instance.ChangeGameState(GameState.Menu);
    }

    public void OnClick_HideMenuPanel()
    {
        menuPanel.SetActive(false);
        menuUI.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.World);
    }

    public void OnClick_ShowStatusPanel()
    {
        var playerData = FindAnyObjectByType<PlayerData>();
        if (playerData == null) return;

        statusPanel.Show();
        statusPanel.Init(playerData);
    }

    public void OnClick_ShowEquipmentPanel()
    {
        equipmentPanel.Show();
    }

    public void OnClick_ShowWeaponStorePanel()
    {
        weaponStorePanel.Show();
    }

    public void OnClick_ShowArmorStorePanel()
    {
        armorStorePanel.Show();
    }

    public void OnClick_ShowAccessaryStorePanel()
    {
        accessaryStorePanel.Show();
    }

    public void OnClick_CloseResultPanel()
    {
        resultUI.SetActive(false);

        // ������ �����ٰ� BattleManager�� �˸�
        BattleManager.Instance.ProceedAfterResult();
    }

    public void UpdatePlayerHP(int current, int max)
    {
        playerHPSlider.maxValue = max;
        playerHPSlider.value = current;
        playerHPText.text = $"{current}/{max}";
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
        // ToString("N0")�� ���ڿ� 1,000���� ,�� ����ش�.
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
