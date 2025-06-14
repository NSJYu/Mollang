using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    // 방향별 스프라이트
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private Vector2 movement;

    void Update()
    {
        // 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal"); // A, D
        movement.y = Input.GetAxisRaw("Vertical");   // W, S

        // 입력이 수직 수평 둘 다 들어온 경우, 우선순위 지정 (예: 수직 우선)
        if (movement.x != 0 && movement.y != 0)
        {
            movement.y = 0; // 대각선 방지
        }

        // 방향에 따라 스프라이트 변경
        if (movement.x > 0)
            spriteRenderer.sprite = spriteRight;
        else if (movement.x < 0)
            spriteRenderer.sprite = spriteLeft;
        else if (movement.y > 0)
            spriteRenderer.sprite = spriteUp;
        else if (movement.y < 0)
            spriteRenderer.sprite = spriteDown;
    }

    void FixedUpdate()
    {
        // 물리 이동 처리
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}