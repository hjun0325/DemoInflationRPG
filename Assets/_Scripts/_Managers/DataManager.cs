using UnityEngine;
using System.IO; // 파일 입출력

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public GameSaveData saveData { get; private set; }
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        LoadGame();
    }

    public void LoadGame()
    {
        if(File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        else
        {
            saveData = new GameSaveData();
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }

    public void UpdateSaveData()
    {
        // GameManager와 PlayerData가 존재하는지 확인 (GameScene에 있을 때만 실행)
        if (GameManager.Instance == null || GameManager.Instance.PlayerData== null) return;
        if (GameManager.Instance.CurrentState == GameState.GameOver) return;

        // 현재 세션 데이터가 없다면 새로 생성
        if (saveData.currentSessionData == null)
        {
            saveData.currentSessionData = new SessionData();
        }

        SessionData session = saveData.currentSessionData;
        PlayerData player = GameManager.Instance.PlayerData;

        session.level = player.level;
        session.maxExp = player.maxExp;
        session.currentExp = player.currentExp;
        session.maxHp = player.baseMaxHp;
        session.atk = player.baseAtk;
        session.def = player.baseDef;
        session.agi = player.baseAgi;
        session.luc = player.baseLuc;
        session.unspentStatPoints = player.unspentStatPoints;
        session.currentGold = player.currentGold;
        session.currentBP = GameManager.Instance.currentBP;

        Vector3 currentPosition = player.transform.position;
        session.playerPosX = currentPosition.x;
        session.playerPosY = currentPosition.y;
    }

    // 새로운 판 시작 시 이전 세션 데이터를 지우는 함수.
    public void ClearSessionData()
    {
        saveData.currentSessionData = null;
    }
}
