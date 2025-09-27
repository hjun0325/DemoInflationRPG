using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    // 현재 구역 정보를 저장할 변수.
    [SerializeField] private ZoneData currentZone;

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

    // 현재 구역의 몬스터 중 하나를 무작위로 반환.
    public MonsterData GetRandomMonsterFromZone()
    {
        if (currentZone == null || currentZone.appearingMonsters.Count == 0)
        {
            Debug.Log("맵 혹은 맵의 몬스터가 없습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, currentZone.appearingMonsters.Count);
        return currentZone.appearingMonsters[randomIndex];
    }

    // 구역이 바뀔 때 호출.
    public void SetCurrentZone(ZoneData newZone)
    {
        currentZone = newZone;
        Debug.Log($"새로운 구역 진입: {currentZone.ZoneName}");
    }
}
