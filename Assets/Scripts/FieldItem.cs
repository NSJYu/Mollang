using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class FieldItem : MonoBehaviour
{
    public Item item;
    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;
    private Rigidbody2D itemRigidbody;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();
        itemRigidbody = GetComponent<Rigidbody2D>();

        // Collider 설정 - 트리거로 설정하여 PlayerInteraction이 감지할 수 있도록
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }

        // Rigidbody2D 설정
        if (itemRigidbody != null)
        {
            itemRigidbody.gravityScale = 0f; // 2D 탑다운 게임이므로 중력 비활성화
            itemRigidbody.drag = 5f; // 드래그 설정으로 자연스러운 움직임
        }
    }

    public void SetItem(Item _item)
    {
        this.item = _item;
        if (spriteRenderer != null && _item != null)
        {
            spriteRenderer.sprite = _item.icon;
            Debug.Log($"FieldItem 스프라이트 설정 완료: {_item.itemName}");
        }

        // 게임 오브젝트 이름을 아이템 이름으로 설정
        if (_item != null)
        {
            gameObject.name = $"FieldItem_{_item.itemName}";
        }
    }

    public Item GetItem()
    {
        if (item == null)
        {
            Debug.LogWarning("FieldItem에 아이템이 설정되지 않았습니다!");
            Destroy(gameObject);
            return null;
        }

        Item itemToReturn = item;
        Debug.Log($"FieldItem에서 아이템 반환: {itemToReturn.itemName}");
        Destroy(gameObject);
        return itemToReturn;
    }

    private void OnValidate()
    {
        // 에디터에서 컴포넌트 자동 설정
        if (Application.isPlaying) return;

        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        if (GetComponent<Rigidbody2D>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = 5f;
        }
    }
}