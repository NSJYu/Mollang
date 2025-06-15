using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public Item item;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private void Awake()
    {
        // 자식 오브젝트에서 컴포넌트를 확실하게 찾아 할당합니다.
        // true 파라미터는 비활성화된 자식 오브젝트도 찾도록 합니다.
        icon = transform.Find("Icon")?.GetComponent<Image>();
        if (icon == null) Debug.LogError(gameObject.name + "에서 Icon을 찾을 수 없습니다.");

        quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
        if (quantityText == null) Debug.LogError(gameObject.name + "에서 QuantityText를 찾을 수 없습니다.");

        UpdateSlotUI(); // 초기 UI 설정
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 왼쪽 마우스 클릭일 때만 반응
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 씬에 있는 Inventory 관리자를 찾아 OnSlotClicked 함수를 호출
            FindObjectOfType<Inventory>()?.OnSlotClicked(this);
        }
    }

    // 아이템 데이터에 따라 UI를 업데이트하는 함수
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

    // 외부에서 이 슬롯의 아이템을 설정하고, 즉시 UI를 업데이트하는 유일한 통로
    public void SetItem(Item newItem)
    {
        item = newItem;
        UpdateSlotUI();
    }
}