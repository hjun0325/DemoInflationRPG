using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private GameObject Visual;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private GameObject cursor;

    private float radius;
    private Vector2 touchPos;

    public void Start()
    {
        radius = background.GetComponent<RectTransform>().sizeDelta.y / 3;
        
        Visual.SetActive(false);
    }

    // 해당 오브젝트가 비활성화 될 때 호출된다.
    private void OnDisable()
    {
        // 커서를 해당 위치로 이동. (커서를 중앙에 두기 위해서)
        cursor.transform.position = touchPos;

        GameManager.instance.joystickDir = Vector2.zero;

        Visual.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Visual.SetActive(true);

        // 터치된 위치로 백그라운드와 커서를 위치시키고 해당 위치 저장.
        background.transform.position = eventData.position;
        cursor.transform.position = eventData.position;
        touchPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDisable();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDir = (eventData.position - touchPos); // 방향 벡터.
        float moveDist = Mathf.Min(touchDir.magnitude, radius); // Cursor 범위 제한.

        // 방향 벡터를 정규화한 후 크기만큼 곱한 위치로 커서 이동.
        Vector2 moveDir = touchDir.normalized;
        Vector2 newPosition = touchPos + moveDir * moveDist;
        cursor.transform.position = newPosition;

        // 플레이어의 방향을 게임 매니저에 넘겨준다.
        GameManager.instance.joystickDir = moveDir;
    }
}
