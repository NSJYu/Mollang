using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector2 moveInput;

    private PlayerControls playerControls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        // 이제 Player 액션 맵만 여기서 직접 제어합니다.
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void Update()
    {
        // Player 액션 맵의 Move 액션에서 값을 읽어옴
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateVisuals()
    {
        if (moveInput.sqrMagnitude > 0.1f)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0) spriteRenderer.sprite = spriteRight;
                else spriteRenderer.sprite = spriteLeft;
            }
            else
            {
                if (moveInput.y > 0) spriteRenderer.sprite = spriteUp;
                else spriteRenderer.sprite = spriteDown;
            }
        }
    }

    /// <summary>
    /// 외부(Inventory.cs)에서 호출하여 플레이어의 게임 플레이 입력을 활성화합니다.
    /// </summary>
    public void EnableGameplayInput()
    {
        playerControls.Player.Enable();
    }

    /// <summary>
    /// 외부(Inventory.cs)에서 호출하여 플레이어의 게임 플레이 입력을 비활성화합니다.
    /// </summary>
    public void DisableGameplayInput()
    {
        playerControls.Player.Disable();
    }
}