using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class ZoneTrigger : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private ZoneData zoneData;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] private Color safeColor = Color.blue;   // 안전
    [SerializeField] private Color cautionColor = Color.yellow; // 주의
    [SerializeField] private Color dangerColor = Color.red;    // 위험

    private void Start()
    {
        SetupBorder();
        SetupLevelText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(zoneData.name);
            MapManager.Instance.SetCurrentZone(zoneData);
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        Gizmos.color = Color.black;

        // Collider의 크기와 동일한 와이어 사각형을 그림
        Vector3 center =
            transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        Gizmos.DrawWireCube(center, collider.size);
    }

    private void SetupBorder()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        Vector3 size = collider.size;
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(-size.x / 2, size.y / 2, 0);
        corners[1] = new Vector3(size.x / 2, size.y / 2, 0);
        corners[2] = new Vector3(size.x / 2, -size.y / 2, 0);
        corners[3] = new Vector3(-size.x / 2, -size.y / 2, 0);

        lineRenderer.positionCount = 5;
        lineRenderer.SetPositions(
            new Vector3[] { corners[0], corners[1], corners[2], corners[3], corners[0] });
        lineRenderer.startWidth = 0.08f;
        lineRenderer.endWidth = 0.08f;
        lineRenderer.useWorldSpace = false; // 오브젝트를 따라 움직이도록 설정
        lineRenderer.startColor = safeColor;
        lineRenderer.endColor = safeColor;
    }

    private void SetupLevelText()
    {
        if (levelText != null && zoneData != null)
        {
            levelText.text = $"적정 Lv.{zoneData.recommendedLevel}";
        }
    }

    public void UpdateBorderColor(int playerLevel)
    {
        if (zoneData == null) return;

        // 적정 레벨과 플레이어 레벨의 차이를 계산
        int levelDifference = zoneData.recommendedLevel - playerLevel;

        if (levelDifference > 10) // 적정 레벨이 10 이상 높으면
        {
            SetBorderColor(dangerColor); // 위험 (빨간색)
        }
        else if (levelDifference > 3) // 적정 레벨이 3~10 높으면
        {
            SetBorderColor(cautionColor); // 주의 (노란색)
        }
        else // 플레이어 레벨이 적정 레벨보다 같거나 높으면
        {
            SetBorderColor(safeColor); // 안전 (파란색)
        }
    }

    private void SetBorderColor(Color color)
    {
        if (lineRenderer == null) return;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
