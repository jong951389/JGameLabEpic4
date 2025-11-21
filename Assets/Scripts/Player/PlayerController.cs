using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("PlayerMove")]
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float jumpForce = 10.0f;

    [SerializeField] float airControl = 0.3f;

    [SerializeField] PlayerInputActions actions;
    Rigidbody2D rb;

    private void Awake()
    {
        actions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }

    void FixedUpdate()
    {
        PlayerMove();
        PlayerJump();
    }

    void PlayerMove()
    {
        // 입력값 읽기 (Vector2)
        Vector2 input = actions.MoveActions.Move.ReadValue<Vector2>();

        // 목표 x 속도
        float targetX = input.x * moveSpeed;

        // y속도가 어느 정도 나가면 공중이라고 판정
        bool isAir = Mathf.Abs(rb.linearVelocity.y) > 0.1f;

        if (isAir)
        {
            //   약간의 관성이 남도록 Lerp 사용 
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, airControl);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
        else
        {
            // 지상에서는 즉시 목표 속도로
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);
        }
    }

    void PlayerJump()
    {
        float jumpInput = actions.MoveActions.Jump.ReadValue<float>();

        // 점프 버튼이 눌렸고, 거의 지상일 때만 점프
        if (jumpInput > 0 && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
