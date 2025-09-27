using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

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
    [SerializeField] private Image monsterImage;
    [SerializeField] private Slider playerHPSlider;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private TMP_Text battleLogText;

    [Header("Result UI")]
    [SerializeField] private GameObject resultUI;
    [SerializeField] private GameObject ButtonPanel;
    //[SerializeField] private CanvasGroup resultCanvasGroup;
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

    public async UniTask PlayRewardAnimationAsync(long startMoney, long gainedMoney, long startExp, long gainedExp, long maxExp, long currentLevel)
    {
        // 두 개의 Task를 각각 생성하고 시작.
        var goldTask = PlayGoldAnimationAsync(startMoney, gainedMoney);
        var expTask = PlayExpAnimationAsync(startExp, gainedExp, maxExp, currentLevel);

        // UniTask.WhenAll을 통해 두 Task가 모두 끝날 때까지 기다림
        await UniTask.WhenAll(goldTask, expTask);

        Debug.Log("모든 보상 연출 완료!");
        ButtonPanel.SetActive(true);
        //resultCanvasGroup.interactable = true;
    }

    // 골드 보상 연출용 비동기 함수.
    private async UniTask PlayGoldAnimationAsync(long startMoney, long gainedMoney)
    {
        float goldAnimDuration = 2.0f; // 골드 연출 시간
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
        // 최종값 보정
        currentMoneyText.text = (startMoney + gainedMoney).ToString("N0");
        plusMoneyText.text = "+0";
    }

    // 경험치 보상 연출용 비동기 함수.
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

            // UI 업데이트.
            resultExpSlider.value = currentDisplayExp;
            expText.text = $"{currentDisplayExp} / {maxDisplayExp}";
            plusExpText.text = $"+ {remainingExp:N0}";
            
            if (currentDisplayExp >= maxDisplayExp)
            {
                currentDisplayLevel++;
                currentDisplayExp -= maxDisplayExp;
                maxDisplayExp = (long)(3 + Mathf.Pow(currentDisplayLevel, 1.5f) * 0.5f);
                resultExpSlider.value = 0; // 바 초기화.
                resultExpSlider.maxValue = maxDisplayExp;
                await UniTask.Delay(100, DelayType.UnscaledDeltaTime);
            }
            await UniTask.Delay((int)(tickSpeed * 500), DelayType.UnscaledDeltaTime);
        }
        resultExpSlider.value = currentDisplayExp; // 최종값 보정
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
        ButtonPanel.SetActive(false);
        //resultCanvasGroup.interactable = false;
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
