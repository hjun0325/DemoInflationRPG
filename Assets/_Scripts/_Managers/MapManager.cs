using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    private List<ZoneTrigger> allZoneTriggers = new List<ZoneTrigger>();
    private PlayerData playerData;

    [SerializeField] private ZoneData defaultZone;
    private ZoneData currentZone;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentZone = defaultZone;
    }

    private void Start()
    {
        allZoneTriggers.AddRange(FindObjectsByType<ZoneTrigger>(FindObjectsSortMode.None));
        playerData = FindAnyObjectByType<PlayerData>();

        // 이벤트 구독
        PlayerData.OnPlayerDataUpdated += UpdateAllZoneBorders;

        // 게임 시작 시, 현재 레벨 기준으로 초기 색상 설정
        UpdateAllZoneBorders();
    }

    private void UpdateAllZoneBorders()
    {
        if (playerData == null) return;

        // 리스트에 있는 모든 ZoneTrigger에게 색상 업데이트를 명령
        foreach (var zone in allZoneTriggers)
        {
            zone.UpdateBorderColor(playerData.level);
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
