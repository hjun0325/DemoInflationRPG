using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(zoneData.name);
            MapManager.Instance.SetCurrentZone(zoneData);
        }
    }

    private void SetupBorder()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
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
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
    }

    private void SetupLevelText()
    {
        if (levelText != null && zoneData != null)
        {
            levelText.text = $"적정 Lv.{zoneData.recommendedLevel}";
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();

        Gizmos.color = Color.black;

        // Collider의 크기와 동일한 와이어 사각형을 그림
        Vector3 center = 
            transform.position + new Vector3(collider.center.x, collider.center.y, 0);
        Gizmos.DrawWireCube(center, collider.size);
    }

    private void UpdateBorderColor(int playerLevel)
    {
        if (zoneData == null) return;
        int levelDifference = zoneData.recommendedLevel - playerLevel;

        if (levelDifference > 100) SetBorderColor(dangerColor);
        else if (levelDifference > 50) SetBorderColor(cautionColor);
        else SetBorderColor(safeColor);
    }

    private void SetBorderColor(Color color)
    {
        if (lineRenderer == null) return;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
