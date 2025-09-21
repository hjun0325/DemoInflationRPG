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
    public Vector2 joystickDir { get; set; } = Vector2.zero; // 플레이어의 이동 방향 설정.

    public GameState CurrentState { get; private set; } // 현재 게임 상태.
    private PlayerData playerData;

    public int currentBP; // 현재 배틀 포인트를 저장할 변수
    private const int STARTING_BP = 30; // 시작 BP를 상수로 정의

    [Header("Encounter System")]
    public float encounterGauge;
    private float maxGauge = 100f;
    private float increaseRate = 20f;

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

    private void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        StartNewRun();
    }

    // 새로운 판을 시작하는 메인 함수.
    public void StartNewRun()
    {
        Debug.Log("새로운 판 시작!");

        // 배틀 포인트 초기화.
        currentBP = STARTING_BP;

        // 플레이어 데이터 초기화.
        if (playerData != null)
        {
            playerData.InitializeForNewRun();
        }

        // UI 업데이트.
        UIManager.instance.UpdateBP(currentBP);
        UIManager.instance.UpdateExp(playerData.currentExp, playerData.maxExp);
        UIManager.instance.UpdateLevel(playerData.level);
        UIManager.instance.UpdateMoney(playerData.currentGold);

        ChangeGameState(GameState.World);
    }

    // 게임의 상태 변경 함수. (관련 UI 및 로직 활성화/비활성화).
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"게임 상태 변경 -> {newState}");

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
                // TODO: 메인 메뉴 UI 활성화
                break;
        }
    }

    // 인카운터 게이지 증가 함수 (PlayerController가 호출할 함수).
    public void AddEncounterValue()
    {
        // 이동하는 동안 게이지를 증가 시킴.
        encounterGauge += increaseRate * Time.deltaTime;

        // 게이지가 최대치가 넘지 않도록.
        encounterGauge = Mathf.Clamp(encounterGauge, 0, maxGauge);

        // UIManager에 게이지 UI 업데이트 요청.
        UIManager.instance.UpdateEncounterGauge(encounterGauge, maxGauge);

        // 몬스터를 만날지 확률 체크.
        CheckForEncounter();
    }

    // 인카운터 체크.
    private void CheckForEncounter()
    {
        if (encounterGauge >= maxGauge)
        {
            Debug.Log("게이지 100%! 몬스터와 전투!");
            StartEncounter(); // 전투 시작.
            return;
        }

        // 게이지가 높을수록 만날 확률 증가.
        float encounterChance = encounterGauge / maxGauge;

        // 매 프레임 확률을 체크. (deltaTime으로 보정).
        if (Random.Range(0f, 1f) < encounterChance * Time.deltaTime)
        {
            Debug.Log("몬스터와 전투!");
            StartEncounter(); // 전투 시작.
        }
    }

    // 인카운터 발생.
    private void StartEncounter()
    {
        encounterGauge = 0f; // 게이지 초기화.

        // 전투 시작 로직 호출.
        ChangeGameState(GameState.Battle);
    }

    // 전투 종료.
    public void EndBattle(bool playerWin)
    {
        Debug.Log("전투 종료!");

        // BP 차감.
        currentBP--;
        UIManager.instance.UpdateBP(currentBP);

        if (playerWin)
        {
            Debug.Log("플레이어가 이겼습니다!");
            // TODO: 보상 적용
        }
        else
        {
            // TODO: 패배 처리
        }

        if (currentBP <= 0)
        {
            EndRun(); // BP가 0이면 판 종료
        }
        else
        {
            // 게임 상태를 다시 '월드'로 전환
            //UIManager.instance.HideBattleUI();
            ChangeGameState(GameState.World);
        }
    }

    public void EndRun()
    {
        Debug.Log("게임 오버! BP를 모두 소진했습니다.");
        // TODO: 게임 오버 UI 표시 및 장비 계승 로직
    }
}