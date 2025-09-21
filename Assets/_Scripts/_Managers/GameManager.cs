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

        currentBP = STARTING_BP;

        if (playerData != null)
        {
            playerData.InitializeForNewRun();
        }

        //UIManager.instance.UpdatePlayerHP(playerData.currentHp, playerData.maxHp);
        UIManager.instance.UpdateBP(currentBP);

        ChangeGameState(GameState.World);
    }

    // ������ ���� ���� �Լ�. (���� UI �� ���� Ȱ��ȭ/��Ȱ��ȭ).
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"���� ���� ���� -> {newState}");

        switch(CurrentState)
        {
            case GameState.World:
                // TODO: �ΰ��� HUD UI Ȱ��ȭ
                // TODO: ���̽�ƽ Ȱ��ȭ
                break;

            case GameState.Battle:
                // TODO: ���̽�ƽ ��Ȱ��ȭ
                // TODO: ���� UI Ȱ��ȭ
                break;

            case GameState.Menu:
                // TODO: ���̽�ƽ ��Ȱ��ȭ
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
        BattleManager.instance.StartBattle();
    }

    // ���� ����.
    public void EndBattle(bool playerWin)
    {
        Debug.Log("���� ����!");

        // BP ����.
        currentBP--;
        //UIManager.instance.UpdateBP(currentBP);

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
            //EndRun(); // BP�� 0�̸� �� ����
        }
        else
        {
            // ���� ���¸� �ٽ� '����'�� ��ȯ
            ChangeGameState(GameState.World);
            // UIManager.Instance.HideBattleUI();
        }
    }

    public void EndRun()
    {
        Debug.Log("���� ����! BP�� ��� �����߽��ϴ�.");
        // TODO: ���� ���� UI ǥ�� �� ��� ��� ����
    }
}