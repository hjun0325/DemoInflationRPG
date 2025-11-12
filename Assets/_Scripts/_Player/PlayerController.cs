using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7;

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CharacterController characterController;

    private Vector2 moveDir;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterController = GetComponent<CharacterController>();
        moveDir = Vector2.zero;
    }

    private void Update()
    {
        // 조이스틱 방향을 가져와 이동 벡터 생성.
        moveDir = GameManager.Instance.JoystickDir;

        // 움직임이 있는 경우.
        if (moveDir != Vector2.zero)
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
            // 이동 방향을 정규화하여 대각선 이동 속도를 보정
            // Time.fixedDeltaTime을 사용하여 물리 프레임에 맞춘 부드러운 이동
            Vector2 nextPosition = rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPosition);
        }
    }
}
