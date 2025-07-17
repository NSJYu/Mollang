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
    private bool isHotbarSlot = false;

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

        // 이 슬롯이 인벤토리 핫바 슬롯인지 확인
        CheckIfHotbarSlot();
    }

    private void CheckIfHotbarSlot()
    {
        Debug.Log($"CheckIfHotbarSlot 호출: {gameObject.name}");

        if (inventory != null && inventory.InventoryHotbarGrid != null)
        {
            isHotbarSlot = transform.IsChildOf(inventory.InventoryHotbarGrid);
            Debug.Log($"슬롯 {gameObject.name}: isHotbarSlot = {isHotbarSlot}");
            if (isHotbarSlot)
            {
                Debug.Log($"✅ 핫바 슬롯으로 인식됨: {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"CheckIfHotbarSlot 실패: inventory={inventory != null}, InventoryHotbarGrid={inventory?.InventoryHotbarGrid != null}");
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

        // 인벤토리 핫바 슬롯이면 메인 핫바에 동기화
        if (isHotbarSlot)
        {
            NotifyHotbarChange();
        }
    }

    private void NotifyHotbarChange()
    {
        Debug.Log($"NotifyHotbarChange 호출: {gameObject.name}, 아이템: {item?.itemName}");

        Hotbar hotbar = FindObjectOfType<Hotbar>();
        if (hotbar == null)
        {
            Debug.LogError("Hotbar 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("inventory가 null입니다!");
            return;
        }

        if (inventory.InventoryHotbarGrid == null)
        {
            Debug.LogError("inventory.InventoryHotbarGrid가 null입니다!");
            return;
        }

        int slotIndex = transform.GetSiblingIndex();
        Debug.Log($"🔄 인벤토리 핫바 슬롯 {slotIndex} 변경됨, 메인 핫바에 알림: {item?.itemName}");
        hotbar.UpdateSlot(slotIndex, item);
    }

    // 아이템이 직접 할당될 때도 동기화 (인벤토리 시스템에서 사용)
    public void SetItemDirect(Item newItem)
    {
        Item oldItem = item;
        item = newItem;
        UpdateSlotUI();

        // 인벤토리 핫바 슬롯이고 실제로 아이템이 변경되었으면 알림
        if (isHotbarSlot && !AreItemsEqual(oldItem, newItem))
        {
            Debug.Log($"ItemSlot.SetItemDirect: 핫바 슬롯 {transform.GetSiblingIndex()} 직접 변경됨");
            NotifyHotbarChange();
        }
    }

    private bool AreItemsEqual(Item item1, Item item2)
    {
        if (item1 == null && item2 == null) return true;
        if (item1 == null || item2 == null) return false;
        return item1.itemName == item2.itemName && item1.quantity == item2.quantity;
    }
}