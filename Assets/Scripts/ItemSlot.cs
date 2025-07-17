using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    private InventoryManager inventory;

    private void Awake()
    {
        icon = transform.Find("Icon")?.GetComponent<Image>();
        if (icon == null) Debug.LogError(gameObject.name + "에서 Icon을 찾을 수 없습니다.");

        quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
        if (quantityText == null) Debug.LogError(gameObject.name + "에서 QuantityText를 찾을 수 없습니다.");

        UpdateSlotUI();
    }

    private void Start()
    {
        // FindObjectOfType으로 InventoryManager 찾기
        inventory = FindObjectOfType<InventoryManager>();
        if (inventory == null) 
        {
            Debug.LogError("ItemSlot이 InventoryManager를 찾을 수 없습니다!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { inventory?.SetHoveredSlot(this); }
    public void OnPointerExit(PointerEventData eventData) { inventory?.SetHoveredSlot(null); }

    public void UpdateSlotUI()
    {
        if (icon == null || quantityText == null) return;
        if (item != null && item.icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = item.icon;
            quantityText.enabled = item.isStackable && item.quantity > 1;
            quantityText.text = item.quantity.ToString();
        }
        else
        {
            icon.gameObject.SetActive(false);
            quantityText.enabled = false;
        }
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateSlotUI();
    }
}