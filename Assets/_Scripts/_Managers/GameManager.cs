using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    World,
    Battle,
    Menu
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Vector2 JoystickDir { get; set; } = Vector2.zero; // �÷��̾��� �̵� ���� ����.
    public GameState CurrentState { get; private set; }
    public PlayerData PlayerData { get; private set; }

    // --- BP ---
    public int currentBP { get; private set; }
    [SerializeField] private int startingBP = 10;

    // --- EncounterGauge ---
    private float encounterCurrentGauge;
    [SerializeField] private float encounterMaxGauge = 100f;
    [SerializeField] private float increaseRate = 20f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� ����.
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε��, OnSceneLoaded �Լ� ȣ��
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;

        PlayerData = FindAnyObjectByType<PlayerData>();
        if(PlayerData == null )
        {
            Debug.LogError("GameScene�� PlayerData�� �����ϴ�!");
            return;
        }

        SessionData sessionData = DataManager.Instance.saveData.currentSessionData;
        if (sessionData != null)
        {
            // ���� �����Ͱ� ������ -> '�̾��ϱ�'
            PlayerData.ApplySessionData(sessionData);
            currentBP = sessionData.currentBP;
        }
        else
        {
            // ���� �����Ͱ� ������ -> '���ο� ��'
            PlayerData.InitializeForNewRun();
            currentBP = startingBP;
        }

        // UI ������Ʈ.
        UIManager.Instance.UpdateBP(currentBP);
        UIManager.Instance.UpdateExp(PlayerData.currentExp, PlayerData.maxExp);
        UIManager.Instance.UpdateLevel(PlayerData.level);
        UIManager.Instance.UpdateMoney(PlayerData.currentGold);

        ChangeGameState(GameState.World);
    }

    // ������ ���� ���� �Լ�. (���� UI �� ���� Ȱ��ȭ/��Ȱ��ȭ).
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"���� ���� ���� -> {newState}");

        switch (CurrentState)
        {
            case GameState.World:
                UIManager.Instance.JoystickUI.SetActive(true);
                Time.timeScale = 1f;
                break;

            case GameState.Battle:
                UIManager.Instance.JoystickUI.SetActive(false);
                Time.timeScale = 0f;
                BattleManager.Instance.StartBattle();
                break;

            case GameState.Menu:
                UIManager.Instance.JoystickUI.SetActive(false);
                Time.timeScale = 1f;
                break;
        }
    }

    // ��ī���� ������ ���� �Լ� (PlayerController�� ȣ���� �Լ�).
    public void AddEncounterValue()
    {
        // �̵��ϴ� ���� �������� ���� ��Ŵ.
        encounterCurrentGauge += increaseRate * Time.deltaTime;

        // �������� �ִ�ġ�� ���� �ʵ���.
        encounterCurrentGauge = Mathf.Clamp(encounterCurrentGauge, 0, encounterMaxGauge);

        // UIManager�� ������ UI ������Ʈ ��û.
        UIManager.Instance.UpdateEncounterGauge(encounterCurrentGauge, encounterMaxGauge);

        // ���͸� ������ Ȯ�� üũ.
        CheckForEncounter();
    }

    // ��ī���� üũ.
    private void CheckForEncounter()
    {
        if (encounterCurrentGauge >= encounterMaxGauge)
        {
            Debug.Log("������ 100%! ���Ϳ� ����!");
            StartEncounter(); // ���� ����.
            return;
        }

        // �������� �������� ���� Ȯ�� ����.
        float encounterChance = encounterCurrentGauge / encounterMaxGauge;

        // �� ������ Ȯ���� üũ. (deltaTime���� ����).
        if (Random.Range(0f, 1f) < encounterChance * Time.deltaTime)
        {
            Debug.Log("���Ϳ� ����!");
            StartEncounter(); // ���� ����.
        }
    }

    // ��ī���� �߻�.
    private void StartEncounter()
    {
        encounterCurrentGauge = 0f; // ������ �ʱ�ȭ.

        // ���� ���� ���� ȣ��.
        ChangeGameState(GameState.Battle);
    }

    // ���� ����.
    public void EndBattle(BattleResult result)
    {
        Debug.Log("���� ����!");

        // BP ����.
        currentBP--;

        if (result.playerWin)
        {
            Debug.Log("�÷��̾ �̰���ϴ�!");
            Debug.Log($"���� ����: ����ġ {result.gainedExp}, ��� {result.gainedGold}");

            PlayerData.HealToFull();
            PlayerData.AddGold(result.gainedGold);
            PlayerData.AddExperience(result.gainedExp);
        }
        else
        {
            int penaltyCost = 2;
            currentBP -= penaltyCost;
            Debug.Log($"�й��Ͽ� BP {penaltyCost}�� �Ҿ����ϴ�!");

            PlayerData.HealToFull();
        }

        DataManager.Instance.SaveGame();
        UIManager.Instance.UpdateBP(currentBP);

        if (currentBP <= 0)
        {
            EndRun(); // BP�� 0�̸� �� ����
        }
        else
        {
            // ���� ���¸� �ٽ� '����'�� ��ȯ
            ChangeGameState(GameState.World);
        }
    }

    public void EndRun()
    {
        Debug.Log("���� ����! BP�� ��� �����߽��ϴ�.");
        // TODO: PlayerData�� ��� ����� DataManager�� ownedItemIDs�� �߰�

        // ���� ������ ���� �� ���� ������ ����.
        DataManager.Instance.ClearSessionData();
        DataManager.Instance.SaveGame();

        SceneManager.LoadScene("MainMenuScene");
    }
}