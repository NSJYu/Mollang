// 🔹 PlayerMovement.cs (New Input System 적용 최종본) 🔹

using UnityEngine;
using UnityEngine.InputSystem; // 새로운 Input System을 위해 추가

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("움직임 설정")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("방향별 스프라이트")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movementInput; // 입력 값을 저장할 변수

    // --- Input System 관련 변수 ---
    private PlayerControls playerControls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Input Actions 초기화
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        // 1. 입력 감지 (새로운 방식)
        HandleInput();

        // 2. 시각적 처리
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        // 3. 물리적 이동 처리
        Move();
    }

    private void HandleInput()
    {
        // Player 액션 맵의 Move 액션에서 Vector2 값을 읽어옴
        movementInput = playerControls.Player.Move.ReadValue<Vector2>();

        // 대각선 이동 방지
        if (Mathf.Abs(movementInput.x) > 0.5f)
        {
            movementInput.y = 0;
        }
    }

    private void UpdateVisuals()
    {
        // 스프라이트 변경 로직 (이전과 거의 동일)
        // movementInput의 크기가 0보다 클 때만 (즉, 움직일 때만) 스프라이트를 바꿈
        if (movementInput.sqrMagnitude > 0.01f)
        {
            if (movementInput.x > 0)
                spriteRenderer.sprite = spriteRight;
            else if (movementInput.x < 0)
                spriteRenderer.sprite = spriteLeft;
            else if (movementInput.y > 0)
                spriteRenderer.sprite = spriteUp;
            else if (movementInput.y < 0)
                spriteRenderer.sprite = spriteDown;
        }
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movementInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}