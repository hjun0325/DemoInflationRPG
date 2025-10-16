using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;

    private Animator animator;
    private CharacterController characterController;

    private Vector3 moveDir;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        moveDir = Vector3.zero;
    }

    private void Update()
    {
        // ���̽�ƽ ������ ������ �̵� ���� ����.
        Vector2 dir = GameManager.Instance.JoystickDir;
        moveDir = new Vector3(dir.x, dir.y, 0);

        // �������� �ִ� ���.
        if (moveDir != Vector3.zero)
        {
            // ��ī���� �������� �ø����� ����.
            GameManager.Instance.AddEncounterValue();

            animator.SetBool("isMoving", true);

            animator.SetFloat("MoveX", moveDir.normalized.x);
            animator.SetFloat("MoveY", moveDir.normalized.y);
        }
        // �������� ���� ���.
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("isMoving"))
        {
            // �̵�.
            characterController.Move(moveDir * Time.deltaTime * moveSpeed);
        }
    }
}
