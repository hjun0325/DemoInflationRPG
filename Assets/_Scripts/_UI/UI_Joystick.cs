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

    // �ش� ������Ʈ�� ��Ȱ��ȭ �� �� ȣ��ȴ�.
    private void OnDisable()
    {
        // Ŀ���� �ش� ��ġ�� �̵�. (Ŀ���� �߾ӿ� �α� ���ؼ�)
        cursor.transform.position = touchPos;

        GameManager.instance.joystickDir = Vector2.zero;

        Visual.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Visual.SetActive(true);

        // ��ġ�� ��ġ�� ��׶���� Ŀ���� ��ġ��Ű�� �ش� ��ġ ����.
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
        Vector2 touchDir = (eventData.position - touchPos); // ���� ����.
        float moveDist = Mathf.Min(touchDir.magnitude, radius); // Cursor ���� ����.

        // ���� ���͸� ����ȭ�� �� ũ�⸸ŭ ���� ��ġ�� Ŀ�� �̵�.
        Vector2 moveDir = touchDir.normalized;
        Vector2 newPosition = touchPos + moveDir * moveDist;
        cursor.transform.position = newPosition;

        // �÷��̾��� ������ ���� �Ŵ����� �Ѱ��ش�.
        GameManager.instance.joystickDir = moveDir;
    }
}
