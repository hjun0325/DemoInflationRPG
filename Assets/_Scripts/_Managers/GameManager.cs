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

    public Vector2 JoystickDir { get; set; } = Vector2.zero; // 플레이어의 이동 방향 설정.
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
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음.
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드시, OnSceneLoaded 함수 호출
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
            Debug.LogError("GameScene에 PlayerData가 없습니다!");
            return;
        }

        SessionData sessionData = DataManager.Instance.saveData.currentSessionData;
        if (sessionData != null)
        {
            // 세션 데이터가 있으면 -> '이어하기'
            PlayerData.ApplySessionData(sessionData);
            currentBP = sessionData.currentBP;
        }
        else
        {
            // 세션 데이터가 없으면 -> '새로운 판'
            PlayerData.InitializeForNewRun();
            currentBP = startingBP;
        }

        // UI 업데이트.
        UIManager.Instance.UpdateBP(currentBP);
        UIManager.Instance.UpdateExp(PlayerData.currentExp, PlayerData.maxExp);
        UIManager.Instance.UpdateLevel(PlayerData.level);
        UIManager.Instance.UpdateMoney(PlayerData.currentGold);

        ChangeGameState(GameState.World);
    }

    // 게임의 상태 변경 함수. (관련 UI 및 로직 활성화/비활성화).
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"게임 상태 변경 -> {newState}");

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

    // 인카운터 게이지 증가 함수 (PlayerController가 호출할 함수).
    public void AddEncounterValue()
    {
        // 이동하는 동안 게이지를 증가 시킴.
        encounterCurrentGauge += increaseRate * Time.deltaTime;

        // 게이지가 최대치가 넘지 않도록.
        encounterCurrentGauge = Mathf.Clamp(encounterCurrentGauge, 0, encounterMaxGauge);

        // UIManager에 게이지 UI 업데이트 요청.
        UIManager.Instance.UpdateEncounterGauge(encounterCurrentGauge, encounterMaxGauge);

        // 몬스터를 만날지 확률 체크.
        CheckForEncounter();
    }

    // 인카운터 체크.
    private void CheckForEncounter()
    {
        if (encounterCurrentGauge >= encounterMaxGauge)
        {
            Debug.Log("게이지 100%! 몬스터와 전투!");
            StartEncounter(); // 전투 시작.
            return;
        }

        // 게이지가 높을수록 만날 확률 증가.
        float encounterChance = encounterCurrentGauge / encounterMaxGauge;

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
        encounterCurrentGauge = 0f; // 게이지 초기화.

        // 전투 시작 로직 호출.
        ChangeGameState(GameState.Battle);
    }

    // 전투 종료.
    public void EndBattle(BattleResult result)
    {
        Debug.Log("전투 종료!");

        // BP 차감.
        currentBP--;

        if (result.playerWin)
        {
            Debug.Log("플레이어가 이겼습니다!");
            Debug.Log($"보상 적용: 경험치 {result.gainedExp}, 골드 {result.gainedGold}");

            PlayerData.HealToFull();
            PlayerData.AddGold(result.gainedGold);
            PlayerData.AddExperience(result.gainedExp);
        }
        else
        {
            int penaltyCost = 2;
            currentBP -= penaltyCost;
            Debug.Log($"패배하여 BP {penaltyCost}를 잃었습니다!");

            PlayerData.HealToFull();
        }

        DataManager.Instance.SaveGame();
        UIManager.Instance.UpdateBP(currentBP);

        if (currentBP <= 0)
        {
            EndRun(); // BP가 0이면 판 종료
        }
        else
        {
            // 게임 상태를 다시 '월드'로 전환
            ChangeGameState(GameState.World);
        }
    }

    public void EndRun()
    {
        Debug.Log("게임 오버! BP를 모두 소진했습니다.");
        // TODO: PlayerData의 장비 목록을 DataManager의 ownedItemIDs에 추가

        // 세션 데이터 삭제 및 영구 데이터 저장.
        DataManager.Instance.ClearSessionData();
        DataManager.Instance.SaveGame();

        SceneManager.LoadScene("MainMenuScene");
    }
}