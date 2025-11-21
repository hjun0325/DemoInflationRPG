using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] public GameObject JoystickUI;

    public int CurrentTargetSlotIndex { get; private set; } = -1;

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
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject blockerPanel;

    [Header("Equipment UI")]
    [SerializeField] private GameObject equipmentUI;
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

    // 전투 보상 연출
    public async UniTask PlayRewardAnimationAsync(long startMoney, long gainedMoney, long startExp, long gainedExp, long maxExp, long currentLevel)
    {
        await UniTask.SwitchToMainThread();

        // 경험치 연출을 시작 전, '총 몇 번' 레벨업할지 미리 계산
        int totalLevelsGained = CalculateTotalLevelUps(startExp, gainedExp, maxExp, currentLevel);

        // 골드 연출과 경험치 연출 Task를 동시에 시작
        var expTask = PlayExpAnimationAsync(startExp, gainedExp, maxExp, currentLevel, totalLevelsGained);
        var goldTask = PlayGoldAnimationAsync(startMoney, gainedMoney);

        // 두 연출이 모두 끝날 때까지 기다림
        await UniTask.WhenAll(goldTask, expTask);

        Debug.Log("모든 보상 연출 완료!");
        closeResultButton.interactable = true;
    }

    // 골드 보상 연출
    private async UniTask PlayGoldAnimationAsync(long startMoney, long gainedMoney)
    {
        float duration = 1.5f; // 골드 연출 시간

        long finalMoney = startMoney + gainedMoney;

        // 보유 골드 증가 애니메이션
        DOTween.To(() => startMoney, x => startMoney = x, finalMoney, duration)
            .OnUpdate(() => currentMoneyText.text = startMoney.ToString("N0"))
            .SetUpdate(true); // Time.timeScale=0일 때도 작동

        // 보상 골드 감소 애니메이션
        await DOTween.To(() => gainedMoney, x => gainedMoney = x, 0, duration)
            .OnUpdate(() => plusMoneyText.text = $"+ {gainedMoney:N0}")
            .SetUpdate(true)
            .AsyncWaitForCompletion(); // 이 애니메이션이 끝날 때까지 대기

        // 애니메이션이 끝난 후 최종 값으로 보정
        currentMoneyText.text = finalMoney.ToString("N0");
        plusMoneyText.text = "+0";
    }

    // 경험치 보상 연출
    private async UniTask PlayExpAnimationAsync(long startExp, long gainedExp, long maxExp, long currentLevel, int totalLevelsGained)
    {
        long remainingExp = gainedExp; // 남은 경험치
        long currentDisplayExp = startExp; // 시작 경험치
        long maxDisplayExp = maxExp; // 맥스 경험치
        long currentDisplayLevel = currentLevel; // 현재 레벨
        long finalLevel = currentLevel + totalLevelsGained; // 최종 레벨

        // --- UI 초기 설정 ---
        levelText.text = currentDisplayLevel.ToString();
        resultExpSlider.value = currentDisplayExp;
        resultExpSlider.maxValue = maxDisplayExp;
        expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
        plusExpText.text = $"+ {remainingExp:N0}";

        if (remainingExp > 0) SoundManager.Instance.PlaySFX("LevelUp");

        // 경험치 바가 차는 시간
        float fillDuration = 0.05f;

        // 현재 레벨업 된 횟수
        int currentLevelStep = 0;

        // 경험치 연출
        while (remainingExp > 0)
        {
            long expToLevelUp = maxDisplayExp - currentDisplayExp;

            // 남은 경험치로 레벨업 가능한 경우
            if (remainingExp >= expToLevelUp)
            {
                long previousRemainingExp = remainingExp;
                remainingExp -= expToLevelUp;

                int nextStep = currentLevelStep + 1;

                bool isPhase1 = (nextStep <= 7); // 1~7렙: 매번 연출
                bool isPhase2 = (nextStep > 7) && ((nextStep - 8) % 5 == 0); // 5배수
                bool isFinalLevel = (currentDisplayLevel + 1 == finalLevel);

                bool showAnimation = isPhase1 || isPhase2 || isFinalLevel;

                if (showAnimation)
                {
                    // 슬라이더 애니메이션
                    var sliderTask = resultExpSlider.DOValue(maxDisplayExp, fillDuration)
                        .SetEase(Ease.Linear)
                        .SetUpdate(true)
                        .AsyncWaitForCompletion()
                        .AsUniTask();

                    // 보상 경험치 애니메이션
                    var plusExpTextTask = DOTween.To(() => previousRemainingExp, x => previousRemainingExp = x, remainingExp, fillDuration)
                        .OnUpdate(() => plusExpText.text = $"+ {previousRemainingExp:N0}")
                        .SetEase(Ease.Linear)
                        .SetUpdate(true)
                        .AsyncWaitForCompletion()
                        .AsUniTask();

                    // 현재 경험치 애니메이션
                    var textTask = DOTween.To(() => currentDisplayExp, x => currentDisplayExp = x, maxDisplayExp, fillDuration)
                        .OnUpdate(() => expText.text = $"{currentDisplayExp} / {maxDisplayExp}")
                        .SetEase(Ease.Linear)
                        .SetUpdate(true)
                        .AsyncWaitForCompletion()
                        .AsUniTask();

                    await UniTask.WhenAll(sliderTask, plusExpTextTask, textTask);
                }

                // 레벨업 데이터 처리
                currentDisplayLevel++;
                currentLevelStep++;
                currentDisplayExp = 0;
                maxDisplayExp = (long)(3 + Mathf.Pow(currentDisplayLevel, 1.5f) * 0.5f);

                levelText.text = currentDisplayLevel.ToString();
                expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
                resultExpSlider.value = 0;
                resultExpSlider.maxValue = maxDisplayExp;

                // 8렙 미만이거나, 5의 배수 레벨업이거나, 마지막 레벨업이면 연출 실행
                if (isPhase1 || isPhase2 || isFinalLevel)
                {
                    SoundManager.Instance.PlaySFX("LevelUp");
                }
            }
            // 마지막 남은 경험치 추가.
            else
            {
                long targetDisplayExp = currentDisplayExp + remainingExp;
                long previousRemainingExp = remainingExp; // 현재 남아있는 경험치 값을 저장
                remainingExp = 0;

                // 슬라이더 애니메이션
                var sliderTask = resultExpSlider.DOValue(targetDisplayExp, fillDuration)
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

                // 보상 경험치 애니메이션
                var plusExpTextTask = DOTween.To(() => previousRemainingExp, x => previousRemainingExp = x, remainingExp, fillDuration)
                    .OnUpdate(() => plusExpText.text = $"+ {previousRemainingExp:N0}")
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

                // 현재 경험치 애니메이션
                var textTask = DOTween.To(() => currentDisplayExp, x => currentDisplayExp = x, targetDisplayExp, fillDuration)
                    .OnUpdate(() => expText.text = $"{currentDisplayExp} / {maxDisplayExp}")
                    .SetEase(Ease.Linear)
                    .SetUpdate(true)
                    .AsyncWaitForCompletion()
                    .AsUniTask();

                await UniTask.WhenAll(sliderTask, plusExpTextTask, textTask);

                currentDisplayExp = targetDisplayExp;
                expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
            }
        }
    }

    // 총 몇 번의 레벨업이 발생하는지 미리 계산하는 함수
    private int CalculateTotalLevelUps(long startExp, long gainedExp, long maxExp, long currentLevel)
    {
        long exp = startExp + gainedExp;
        long max = maxExp;
        int levelCount = 0;

        while (exp >= max)
        {
            exp -= max;
            currentLevel++;
            levelCount++;
            max = (long)(3 + Mathf.Pow(currentLevel, 1.5f) * 0.5f);
        }
        return levelCount;
    }

    public void ShowBattleUI(PlayerData player, MonsterData monster, int monsterMaxHP)
    {
        battleUI.SetActive(true);

        monsterImage.sprite = monster.monsterIcon;
        UpdatePlayerHP(player.currentHp, player.TotalMaxHp);
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

    public void OnClick_OpenOptionsPanel()
    {        
        blockerPanel.SetActive(true);
        optionsPanel.SetActive(true);
    }

    public void OnClick_CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        blockerPanel.SetActive(false);
    }

    public void OnClick_LoadMainMenuScene()
    {
        Time.timeScale = 1f;
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
        equipmentUI.SetActive(true);
        equipmentPanel.Show();
    }

    public void OnClick_CloseEquipmentPanel()
    {
        equipmentUI.SetActive(false);
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

        // 보상이 끝났다고 BattleManager에 알림
        BattleManager.Instance.ProceedAfterResult();
    }

    public void UpdatePlayerHP(int current, int max)
    {
        playerHPSlider.maxValue = max;
        playerHPSlider.value = current;
        playerHPText.text = current < 0 ? $"{0}/{max}" : $"{current}/{max}";
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
