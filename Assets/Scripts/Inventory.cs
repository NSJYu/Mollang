using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [Header("UI 설정 (직접 연결)")]
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private GameObject cursorItemObject;
    [SerializeField] private Transform inventoryHotbarGrid;

    [Header("플레이어 참조")]
    [SerializeField] private PlayerMovement playerMovement;

    private Image cursorItemIcon;
    private TextMeshProUGUI cursorQuantityText;
    private bool isOpen = false;
    private Item liftedItem;
    private ItemSlot originalSlot;
    private PlayerControls playerControls;
    private Hotbar mainHotbar;

    private void Awake()
    {
        playerControls = new PlayerControls();
        if (cursorItemObject != null)
        {
            cursorItemIcon = cursorItemObject.GetComponent<Image>();
            cursorQuantityText = cursorItemObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        // Inspector에서 플레이어 연결을 깜빡했다면, 씬에서 직접 찾기
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if(playerMovement == null) Debug.LogError("씬에 PlayerMovement 스크립트가 없습니다!");
        }
    }

    private void OnEnable()
    {
        // InventoryManager는 UI 입력만 담당합니다.
        playerControls.UI.Enable();
        playerControls.UI.OpenInventory.performed += ToggleInventory;
    }

    private void OnDisable()
    {
        playerControls.UI.Disable();
        playerControls.UI.OpenInventory.performed -= ToggleInventory;
    }

    private void Start()
    {
        if (inventoryUIPanel != null) inventoryUIPanel.SetActive(false);
        if (cursorItemObject != null) cursorItemObject.SetActive(false);

        mainHotbar = FindObjectOfType<Hotbar>();
        if (mainHotbar == null) Debug.LogError("씬에서 Hotbar 스크립트를 찾을 수 없습니다!");
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryUIPanel == null || playerMovement == null) return;

        isOpen = !isOpen;
        inventoryUIPanel.SetActive(isOpen);

        // 액션 맵 전환 로직
        if (isOpen)
        {
            // 인벤토리가 열리면, 플레이어 움직임 입력을 끈다.
            playerMovement.DisableGameplayInput();
            SyncHotbarToInventory();
        }
        else
        {
            // 인벤토리가 닫히면, 플레이어 움직임 입력을 다시 켠다.
            playerMovement.EnableGameplayInput();
            // 닫을 때는 실시간 동기화가 이미 다 처리했으므로 별도 동기화 필요 없음
        }

        // 인벤토리를 닫을 때 들고 있던 아이템이 있다면 취소
        if (!isOpen && liftedItem != null)
        {
            CancelLift();
        }
    }

    private void Update()
    {
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
        if (!isOpen) return;
        ItemSlot prevOriginalSlot = originalSlot;

        if (liftedItem == null)
        {
            if (clickedSlot.item != null)
            {
                liftedItem = clickedSlot.item;
                originalSlot = clickedSlot;
                clickedSlot.SetItem(null);
            }
        }
        else
        {
            if (clickedSlot.item != null)
            {
                Item itemInNewSlot = clickedSlot.item;
                clickedSlot.SetItem(liftedItem);
                liftedItem = itemInNewSlot;
                originalSlot = clickedSlot;
            }
            else
            {
                clickedSlot.SetItem(liftedItem);
                liftedItem = null;
                originalSlot = null;
            }
        }

        UpdateCursorUI();

        // 데이터가 변경된 슬롯들을 확인하고 메인 핫바에 즉시 반영
        CheckAndSyncHotbar(clickedSlot);
        if (prevOriginalSlot != null && prevOriginalSlot != clickedSlot)
        {
            CheckAndSyncHotbar(prevOriginalSlot);
        }
    }

    private void CheckAndSyncHotbar(ItemSlot changedSlot)
    {
        if (changedSlot == null || inventoryHotbarGrid == null || mainHotbar == null) return;

        // 변경된 슬롯이 인벤토리 안의 핫바 슬롯인지 확인
        if (changedSlot.transform.parent == inventoryHotbarGrid)
        {
            int slotIndex = changedSlot.transform.GetSiblingIndex();
            mainHotbar.UpdateSlot(slotIndex, changedSlot.item);
        }
    }

    // 인벤토리를 열 때, 메인 핫바의 데이터를 인벤토리 핫바로 복사
    private void SyncHotbarToInventory()
    {
        if (mainHotbar == null || inventoryHotbarGrid == null) return;
        for (int i = 0; i < mainHotbar.hotbarSlots.Length; i++)
        {
            ItemSlot inventorySlot = inventoryHotbarGrid.GetChild(i)?.GetComponent<ItemSlot>();
            if (inventorySlot != null)
            {
                inventorySlot.SetItem(mainHotbar.hotbarSlots[i].item);
            }
        }
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
        CheckAndSyncHotbar(originalSlot); // 취소 시에도 동기화
        liftedItem = null;
        originalSlot = null;
        UpdateCursorUI();
    }
}