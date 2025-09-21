using Cysharp.Threading.Tasks;
using UnityEngine;

public enum GameState
{
    World,
    Battle,
    Menu
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private GameObject joystickUI;
    public Vector2 joystickDir { get; set; } = Vector2.zero; // �÷��̾��� �̵� ���� ����.

    public GameState CurrentState { get; private set; } // ���� ���� ����.
    private PlayerData playerData;

    public int currentBP; // ���� ��Ʋ ����Ʈ�� ������ ����
    private const int STARTING_BP = 30; // ���� BP�� ����� ����

    [Header("Encounter System")]
    public float encounterGauge;
    private float maxGauge = 100f;
    private float increaseRate = 20f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� �ı����� ����.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        StartNewRun();
    }

    // ���ο� ���� �����ϴ� ���� �Լ�.
    public void StartNewRun()
    {
        Debug.Log("���ο� �� ����!");

        // ��Ʋ ����Ʈ �ʱ�ȭ.
        currentBP = STARTING_BP;

        // �÷��̾� ������ �ʱ�ȭ.
        if (playerData != null)
        {
            playerData.InitializeForNewRun();
        }

        // UI ������Ʈ.
        UIManager.instance.UpdateBP(currentBP);
        UIManager.instance.UpdateExp(playerData.currentExp, playerData.maxExp);
        UIManager.instance.UpdateLevel(playerData.level);
        UIManager.instance.UpdateMoney(playerData.currentGold);

        ChangeGameState(GameState.World);
    }

    // ������ ���� ���� �Լ�. (���� UI �� ���� Ȱ��ȭ/��Ȱ��ȭ).
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"���� ���� ���� -> {newState}");

        Time.timeScale = 1f;

        switch (CurrentState)
        {
            case GameState.World:
                joystickUI.SetActive(true);
                break;

            case GameState.Battle:
                joystickUI.SetActive(false);
                Time.timeScale = 0f;
                BattleManager.instance.StartBattle();
                break;

            case GameState.Menu:
                joystickUI.SetActive(false);
                // TODO: ���� �޴� UI Ȱ��ȭ
                break;
        }
    }

    // ��ī���� ������ ���� �Լ� (PlayerController�� ȣ���� �Լ�).
    public void AddEncounterValue()
    {
        // �̵��ϴ� ���� �������� ���� ��Ŵ.
        encounterGauge += increaseRate * Time.deltaTime;

        // �������� �ִ�ġ�� ���� �ʵ���.
        encounterGauge = Mathf.Clamp(encounterGauge, 0, maxGauge);

        // UIManager�� ������ UI ������Ʈ ��û.
        UIManager.instance.UpdateEncounterGauge(encounterGauge, maxGauge);

        // ���͸� ������ Ȯ�� üũ.
        CheckForEncounter();
    }

    // ��ī���� üũ.
    private void CheckForEncounter()
    {
        if (encounterGauge >= maxGauge)
        {
            Debug.Log("������ 100%! ���Ϳ� ����!");
            StartEncounter(); // ���� ����.
            return;
        }

        // �������� �������� ���� Ȯ�� ����.
        float encounterChance = encounterGauge / maxGauge;

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
        encounterGauge = 0f; // ������ �ʱ�ȭ.

        // ���� ���� ���� ȣ��.
        ChangeGameState(GameState.Battle);
    }

    // ���� ����.
    public void EndBattle(bool playerWin)
    {
        Debug.Log("���� ����!");

        // BP ����.
        currentBP--;
        UIManager.instance.UpdateBP(currentBP);

        if (playerWin)
        {
            Debug.Log("�÷��̾ �̰���ϴ�!");
            // TODO: ���� ����
        }
        else
        {
            // TODO: �й� ó��
        }

        if (currentBP <= 0)
        {
            EndRun(); // BP�� 0�̸� �� ����
        }
        else
        {
            // ���� ���¸� �ٽ� '����'�� ��ȯ
            //UIManager.instance.HideBattleUI();
            ChangeGameState(GameState.World);
        }
    }

    public void EndRun()
    {
        Debug.Log("���� ����! BP�� ��� �����߽��ϴ�.");
        // TODO: ���� ���� UI ǥ�� �� ��� ��� ����
    }
}