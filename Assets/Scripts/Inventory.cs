using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // IsPointerOverGameObject를 위해 추가

public class Inventory : MonoBehaviour
{
    [Header("UI 설정 (직접 연결)")]
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private GameObject cursorItemObject;

    private Image cursorItemIcon;
    private TextMeshProUGUI cursorQuantityText;

    private bool isOpen = false;
    private Item liftedItem;
    private ItemSlot originalSlot;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        if (cursorItemObject != null)
        {
            cursorItemIcon = cursorItemObject.GetComponent<Image>();
            cursorQuantityText = cursorItemObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.UI.OpenInventory.performed += ToggleInventory;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.UI.OpenInventory.performed -= ToggleInventory;
    }

    private void Start()
    {
        if (inventoryUIPanel != null) inventoryUIPanel.SetActive(false);
        if (cursorItemObject != null) cursorItemObject.SetActive(false);
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryUIPanel == null) return;
        isOpen = !isOpen;
        inventoryUIPanel.SetActive(isOpen);
        if (!isOpen && liftedItem != null) CancelLift();
    }

    private void Update()
    {
        // 인벤토리가 열려있고, 아이템을 들고 있을 때 커서 아이콘이 마우스를 따라감
        if (isOpen && liftedItem != null && cursorItemObject != null)
        {
            (cursorItemObject.transform as RectTransform).position = Mouse.current.position.ReadValue();
        }

        // 인벤토리 바깥을 클릭했을 때 들고 있던 아이템을 되돌림
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            if (liftedItem != null)
            {
                CancelLift();
            }
        }
    }

    public void OnSlotClicked(ItemSlot clickedSlot)
    {
        // 1. 손에 아이템이 없을 때
        if (liftedItem == null)
        {
            if (clickedSlot.item != null)
            {
                // 아이템 들어올리기
                liftedItem = clickedSlot.item;
                originalSlot = clickedSlot;
                clickedSlot.SetItem(null);
            }
        }
        // 2. 손에 아이템이 있을 때
        else
        {
            if (clickedSlot.item != null) // 다른 아이템이 있는 슬롯이라면 (아이템 교환)
            {
                Item itemInNewSlot = clickedSlot.item;
                clickedSlot.SetItem(liftedItem);
                liftedItem = itemInNewSlot;
                originalSlot = clickedSlot;
            }
            else // 빈 슬롯이라면 (아이템 내려놓기)
            {
                clickedSlot.SetItem(liftedItem);
                liftedItem = null;
                originalSlot = null;
            }
        }
        UpdateCursorUI();
    }

    private void UpdateCursorUI()
    {
        if (cursorItemObject == null) return;

        if (liftedItem != null)
        {
            cursorItemObject.SetActive(true);
            cursorItemIcon.sprite = liftedItem.icon;
            cursorQuantityText.enabled = liftedItem.isStackable && liftedItem.quantity > 1;
            cursorQuantityText.text = liftedItem.quantity.ToString();
        }
        else
        {
            cursorItemObject.SetActive(false);
        }
    }

    private void CancelLift()
    {
        if (originalSlot == null) return;
        originalSlot.SetItem(liftedItem);
        liftedItem = null;
        originalSlot = null;
        UpdateCursorUI();
    }
}