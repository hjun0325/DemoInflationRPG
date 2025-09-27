using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    // ���� ���� ������ ������ ����.
    [SerializeField] private ZoneData currentZone;

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

    // ���� ������ ���� �� �ϳ��� �������� ��ȯ.
    public MonsterData GetRandomMonsterFromZone()
    {
        if (currentZone == null || currentZone.appearingMonsters.Count == 0)
        {
            Debug.Log("�� Ȥ�� ���� ���Ͱ� �����ϴ�.");
            return null;
        }

        int randomIndex = Random.Range(0, currentZone.appearingMonsters.Count);
        return currentZone.appearingMonsters[randomIndex];
    }

    // ������ �ٲ� �� ȣ��.
    public void SetCurrentZone(ZoneData newZone)
    {
        currentZone = newZone;
        Debug.Log($"���ο� ���� ����: {currentZone.ZoneName}");
    }
}
