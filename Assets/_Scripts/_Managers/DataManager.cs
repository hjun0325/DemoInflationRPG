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
            Debug.Log("세이브 데이터 로드 완료!");
        }
        else
        {
            saveData = new GameSaveData();
            Debug.Log("새로운 세이브 데이터 생성!");
        }
    }

    public void SaveGame()
    {
        UpdateSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"게임 저장 완료: {savePath}");
    }

    public void UpdateSaveData()
    {
        // GameManager와 PlayerData가 존재하는지 확인 (GameScene에 있을 때만 실행)
        if (GameManager.Instance == null || GameManager.Instance.PlayerData== null) return;

        // 현재 세션 데이터가 없다면 새로 생성
        if (saveData.currentSessionData == null)
        {
            saveData.currentSessionData = new SessionData();
        }

        SessionData session = saveData.currentSessionData;
        PlayerData player = GameManager.Instance.PlayerData;

        session.level = player.level;
        session.currentExp = player.currentExp;
        session.maxHp = player.maxHp;
        session.atk = player.atk;
        session.def = player.def;
        session.agi = player.agi;
        session.luc = player.luc;
        session.unspentStatPoints = player.unspentStatPoints;
        session.currentGold = player.currentGold;
        session.currentBP = GameManager.Instance.currentBP;
    }

    // 새로운 판 시작 시 이전 세션 데이터를 지우는 함수.
    public void ClearSessionData()
    {
        saveData.currentSessionData = null;
    }
}
