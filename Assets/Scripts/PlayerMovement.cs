using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [Header("방향별 스프라이트")] public Sprite spriteUp, spriteDown, spriteLeft, spriteRight;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private PlayerControls playerControls;
    private void Awake() { rb = GetComponent<Rigidbody2D>(); spriteRenderer = GetComponent<SpriteRenderer>(); playerControls = PlayerControlsManager.playerControls; }
    private void OnEnable() { playerControls?.Player.Enable(); }
    private void OnDisable() { playerControls?.Player.Disable(); }
    private void Update() { if (playerControls == null) return; moveInput = playerControls.Player.Move.ReadValue<Vector2>(); UpdateVisuals(); }
    private void FixedUpdate() { if (playerControls == null) return; rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime); }
    private void UpdateVisuals() { if (moveInput.sqrMagnitude > 0.1f) { if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y)) { if (moveInput.x > 0) spriteRenderer.sprite = spriteRight; else spriteRenderer.sprite = spriteLeft; } else { if (moveInput.y > 0) spriteRenderer.sprite = spriteUp; else spriteRenderer.sprite = spriteDown; } } }
    public void EnableGameplayInput() { playerControls?.Player.Enable(); }
    public void DisableGameplayInput() { playerControls?.Player.Disable(); }
}