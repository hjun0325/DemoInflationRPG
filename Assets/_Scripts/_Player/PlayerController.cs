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
        // 조이스틱 방향을 가져와 이동 벡터 생성.
        Vector2 dir = GameManager.Instance.JoystickDir;
        moveDir = new Vector3(dir.x, dir.y, 0);

        // 움직임이 있는 경우.
        if (moveDir != Vector3.zero)
        {
            // 인카운터 게이지를 올리도록 보고.
            GameManager.Instance.AddEncounterValue();

            animator.SetBool("isMoving", true);

            animator.SetFloat("MoveX", moveDir.normalized.x);
            animator.SetFloat("MoveY", moveDir.normalized.y);
        }
        // 움직임이 없는 경우.
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("isMoving"))
        {
            // 이동.
            characterController.Move(moveDir * Time.deltaTime * moveSpeed);
        }
    }
}
