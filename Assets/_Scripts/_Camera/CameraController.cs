using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target; // ���� ��� (�÷��̾�).
    [SerializeField, Range(0, 1)]
    private float softZoneWidth = 0.40f; // ȭ�� ���� ũ�� ��� ����Ʈ ���� ���� (40%).
    [SerializeField, Range(0, 1)]
    private float softZoneHeight = 0.40f; // ȭ�� ���� ũ�� ��� ����Ʈ ���� ���� (40%).
    
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ī�޶��� ���� ��ġ.
        Vector3 cameraPosition = transform.position;

        // ī�޶� ȭ�� �ʺ�.
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // ����Ʈ �� ����.
        float minX = transform.position.x - (cameraWidth / 2 * softZoneWidth);
        float maxX = transform.position.x + (cameraWidth / 2 * softZoneWidth);
        float minY = transform.position.y - (cameraHeight / 2 * softZoneHeight);
        float maxY = transform.position.y + (cameraHeight / 2 * softZoneHeight);

        // �÷��̾ ����Ʈ ���� �������, ī�޶� ��ġ�� ����.
        Vector3 targetPosition = target.position;
        if (targetPosition.x < minX)
        {
            cameraPosition.x = targetPosition.x + (cameraWidth / 2 * softZoneWidth);
        }
        else if (targetPosition.x > maxX)
        {
            cameraPosition.x = targetPosition.x - (cameraWidth / 2 * softZoneWidth);
        }

        if (targetPosition.y < minY)
        {
            cameraPosition.y = targetPosition.y + (cameraHeight / 2 * softZoneHeight);
        }
        else if (targetPosition.y > maxY)
        {
            cameraPosition.y = targetPosition.y - (cameraHeight / 2 * softZoneHeight);
        }

        // ���� ��ġ�� ī�޶� �̵�.
        transform.position = cameraPosition;
    }

}
