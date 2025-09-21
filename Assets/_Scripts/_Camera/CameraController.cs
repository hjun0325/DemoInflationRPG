using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target; // 따라갈 대상 (플레이어).
    [SerializeField, Range(0, 1)]
    private float softZoneWidth = 0.40f; // 화면 가로 크기 대비 소프트 존의 비율 (40%).
    [SerializeField, Range(0, 1)]
    private float softZoneHeight = 0.40f; // 화면 세로 크기 대비 소프트 존의 비율 (40%).
    
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 카메라의 현재 위치.
        Vector3 cameraPosition = transform.position;

        // 카메라 화면 너비.
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // 소프트 존 설정.
        float minX = transform.position.x - (cameraWidth / 2 * softZoneWidth);
        float maxX = transform.position.x + (cameraWidth / 2 * softZoneWidth);
        float minY = transform.position.y - (cameraHeight / 2 * softZoneHeight);
        float maxY = transform.position.y + (cameraHeight / 2 * softZoneHeight);

        // 플레이어가 소프트 존을 벗어났으면, 카메라 위치를 수정.
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

        // 최종 위치로 카메라 이동.
        transform.position = cameraPosition;
    }

}
