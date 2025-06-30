using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class FieldItem : MonoBehaviour
{
    public Item item;
    private SpriteRenderer spriteRenderer;
    private void Awake() { spriteRenderer = GetComponent<SpriteRenderer>(); }
    public void SetItem(Item _item) { this.item = _item; if (spriteRenderer != null && _item != null) spriteRenderer.sprite = _item.icon; }
    public Item GetItem() { Destroy(gameObject); return item; }
}