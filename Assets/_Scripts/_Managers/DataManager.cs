using UnityEngine;
using System.IO; // ���� �����

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
            Debug.Log("���̺� ������ �ε� �Ϸ�!");
        }
        else
        {
            saveData = new GameSaveData();
            Debug.Log("���ο� ���̺� ������ ����!");
        }
    }

    public void SaveGame()
    {
        UpdateSaveData();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"���� ���� �Ϸ�: {savePath}");
    }

    public void UpdateSaveData()
    {
        // GameManager�� PlayerData�� �����ϴ��� Ȯ�� (GameScene�� ���� ���� ����)
        if (GameManager.Instance == null || GameManager.Instance.PlayerData== null) return;

        // ���� ���� �����Ͱ� ���ٸ� ���� ����
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

    // ���ο� �� ���� �� ���� ���� �����͸� ����� �Լ�.
    public void ClearSessionData()
    {
        saveData.currentSessionData = null;
    }
}
