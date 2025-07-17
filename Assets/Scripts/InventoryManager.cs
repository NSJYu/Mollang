using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    // 싱글톤 패턴
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();
                if (_instance == null)
                {
                    Debug.LogError("InventoryManager가 씬에 없습니다! InventoryManager 컴포넌트가 있는 GameObject를 씬에 추가해주세요.");
                }
            }
            return _instance;
        }
        private set { _instance = value; }
    }
    private static InventoryManager _instance;

    [Header("UI 설정")]
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private GameObject cursorItemObject;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Transform inventoryHotbarGrid;

    [Header("드롭 존 UI")]
    [SerializeField] private GameObject dropZoneUIObject;
    [SerializeField] private Image dropZoneBackground;
    [SerializeField] private Image dropZoneIcon;
    [SerializeField] private TextMeshProUGUI dropZoneText;

    [Header("기타 설정")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject fieldItemPrefab;

    [Header("디버그 설정")]
    [SerializeField] private bool clearAllSlotsOnStart = false; // 시작 시 모든 슬롯 초기화
    [SerializeField] private bool addTestItemsOnStart = false; // 시작 시 테스트 아이템 추가

    // Private fields
    private Image cursorItemIcon;
    private TextMeshProUGUI cursorQuantityText;
    private bool isOpen = false;
    private Item liftedItem;
    private ItemSlot originalSlot;
    // private Hotbar mainHotbar; // 주석 처리 - Hotbar 클래스 참조 제거
    private ItemSlot hoveredSlot;
    private List<ItemSlot> allInventorySlots = new List<ItemSlot>();
    private PlayerControls playerControls;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (_instance == null)
        {
            _instance = this;
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        playerControls = PlayerControlsManager.playerControls;

        if (cursorItemObject != null)
        {
            cursorItemIcon = cursorItemObject.GetComponent<Image>();
            cursorQuantityText = cursorItemObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();

        if (inventoryGrid != null)
        {
            var inventorySlots = inventoryGrid.GetComponentsInChildren<ItemSlot>(true);
            allInventorySlots.AddRange(inventorySlots);
        }

        if (inventoryHotbarGrid != null)
        {
            var hotbarSlots = inventoryHotbarGrid.GetComponentsInChildren<ItemSlot>(true);
            allInventorySlots.AddRange(hotbarSlots);
        }

        if (dropZoneUIObject != null)
            dropZoneUIObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.OpenInventory.performed += ToggleInventory;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Disable();
            playerControls.UI.OpenInventory.performed -= ToggleInventory;
        }
    }

    private void Start()
    {
        if (inventoryUIPanel != null)
            inventoryUIPanel.SetActive(false);

        if (cursorItemObject != null)
            cursorItemObject.SetActive(false);

        // mainHotbar = FindObjectOfType<Hotbar>(); // 주석 처리 - Hotbar 참조 제거

        // 조건부 슬롯 초기화
        if (clearAllSlotsOnStart)
        {
            ClearAllSlots();
        }

        // 조건부 테스트 아이템 추가
        if (addTestItemsOnStart)
        {
            AddTestItems();
        }

        CheckRequiredSettings();
    }

    private void ClearAllSlots()
    {
        Debug.Log("[INIT] 모든 슬롯 초기화 중...");

        foreach (ItemSlot slot in allInventorySlots)
        {
            if (slot != null && slot.item != null)
            {
                Debug.Log($"[INIT] 슬롯 {slot.name}에서 아이템 {slot.item.itemName} 제거");
                slot.item = null;
                slot.UpdateSlotUI();
            }
        }

        // lifted 상태도 초기화
        liftedItem = null;
        originalSlot = null;
        UpdateCursorUI();

        Debug.Log("[INIT] 슬롯 초기화 완료");
    }

    private void CheckRequiredSettings()
    {
        bool hasErrors = false;

        if (fieldItemPrefab == null)
        {
            Debug.LogError("[Inventory] Field Item Prefab이 설정되지 않았습니다!");
            hasErrors = true;
        }

        if (playerMovement == null)
        {
            Debug.LogError("[Inventory] Player Movement가 설정되지 않았습니다!");
            hasErrors = true;
        }

        if (inventoryUIPanel == null)
        {
            Debug.LogError("[Inventory] Inventory UI Panel이 설정되지 않았습니다!");
            hasErrors = true;
        }

        if (!hasErrors)
        {
            Debug.Log("[Inventory] 모든 필수 설정이 완료되었습니다!");
        }
    }

    public void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryUIPanel == null || playerMovement == null) return;

        isOpen = !isOpen;
        inventoryUIPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerMovement.DisableGameplayInput();
            SyncHotbarToInventory();
        }
        else
        {
            playerMovement.EnableGameplayInput();
            if (liftedItem != null) CancelLift();
        }
    }

    private void Update()
    {
        if (!isOpen) return;

        // Input System 체크
        if (Mouse.current == null || Keyboard.current == null)
            return;

        // Q키로 hover된 슬롯의 아이템 드롭
        if (Keyboard.current.qKey.wasPressedThisFrame && hoveredSlot != null && hoveredSlot.item != null && liftedItem == null)
        {
            Debug.Log($"[Q DROP] Q키로 아이템 드롭: {hoveredSlot.item.itemName}");
            Item itemToDrop = hoveredSlot.item;
            hoveredSlot.item = null;
            hoveredSlot.UpdateSlotUI();
            CheckAndSyncHotbar(hoveredSlot);
            DropItemToWorld(itemToDrop);
        }

        // 커서 아이템 위치 업데이트
        if (liftedItem != null && cursorItemObject != null)
        {
            (cursorItemObject.transform as RectTransform).position = Mouse.current.position.ReadValue();
        }

        // 마우스 클릭 처리 - 클릭-클릭 방식
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ItemSlot clickedSlot = GetSlotAtMousePosition();

            if (clickedSlot != null)
            {
                // 슬롯 클릭
                OnSlotClicked(clickedSlot);
            }
            else if (liftedItem != null)
            {
                // 빈 공간 클릭시 월드에 드롭 또는 원래 자리로 되돌리기
                bool isOverUI = IsMouseOverInventoryPanel();
                Debug.Log($"[CLICK] 빈 공간 클릭 - UI 위인가? {isOverUI}, 마우스 위치: {Mouse.current.position.ReadValue()}");

                if (!isOverUI)
                {
                    // UI 밖 클릭시 월드에 드롭
                    Debug.Log($"[DROP] UI 밖 클릭 - 아이템 월드에 드롭: {liftedItem.itemName}");
                    DropItemToWorld(liftedItem);
                    if (originalSlot != null)
                    {
                        originalSlot.item = null;
                        originalSlot.UpdateSlotUI();
                        CheckAndSyncHotbar(originalSlot);
                    }
                    liftedItem = null;
                    originalSlot = null;
                    UpdateCursorUI();
                }
                else
                {
                    // UI 내 빈 공간 클릭시 원래 자리로 되돌리기
                    Debug.Log($"[CANCEL] UI 내 빈 공간 클릭 - 아이템 원래 자리로 되돌리기");
                    CancelLift();
                }
            }
        }
    }

    private ItemSlot GetSlotAtMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 모든 슬롯을 체크
        foreach (ItemSlot slot in allInventorySlots)
        {
            if (slot != null && slot.gameObject.activeInHierarchy)
            {
                RectTransform slotRect = slot.GetComponent<RectTransform>();
                if (slotRect != null)
                {
                    Canvas canvas = slotRect.GetComponentInParent<Canvas>();
                    Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

                    if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePos, cam))
                    {
                        return slot;
                    }
                }
            }
        }

        return null;
    }

    private bool IsMouseOverInventoryPanel()
    {
        // 인벤토리가 비활성화되어 있으면 무조건 UI 밖
        if (inventoryUIPanel == null || !inventoryUIPanel.activeInHierarchy)
        {
            Debug.Log($"[UI CHECK] 인벤토리 패널이 비활성화됨 - UI 밖으로 판정");
            return false;
        }

        // 더 간단한 방법: 마우스 위치가 화면의 특정 영역에 있는지만 체크
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 화면 해상도 기준으로 중앙 영역을 인벤토리 UI로 가정 (임시)
        // 실제로는 inventoryUIPanel의 RectTransform을 정확히 계산해야 함
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 인벤토리 UI가 보통 화면 중앙에 위치한다고 가정
        // 좌우 25% 여백, 상하 15% 여백을 두고 UI 영역 설정
        float horizontalMargin = 0.25f;
        float verticalMargin = 0.15f;
        float uiLeft = screenWidth * horizontalMargin;
        float uiRight = screenWidth * (1 - horizontalMargin);
        float uiBottom = screenHeight * verticalMargin;
        float uiTop = screenHeight * (1 - verticalMargin);

        bool isOverUI = mousePos.x >= uiLeft && mousePos.x <= uiRight &&
                       mousePos.y >= uiBottom && mousePos.y <= uiTop;

        Debug.Log($"[UI CHECK] 간단 계산 - 마우스: {mousePos}, 화면: {screenWidth}x{screenHeight}, UI 영역: ({uiLeft}, {uiBottom}) ~ ({uiRight}, {uiTop}), UI 위: {isOverUI}");

        return isOverUI;
    }

    private void HandleMouseRelease()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool shouldDropToWorld = true;

        Debug.Log($"[DROP] 마우스 릴리즈 - 위치: {mousePos}, 아이템: {liftedItem.itemName}");

        // 인벤토리 패널 내부인지 체크
        if (IsMouseOverInventoryPanel())
        {
            Debug.Log($"[DROP] 인벤토리 패널 위");

            if (hoveredSlot != null)
            {
                // 슬롯에 배치
                Debug.Log($"[DROP] 슬롯에 배치: {hoveredSlot.name}");
                OnSlotClicked(hoveredSlot);
                shouldDropToWorld = false;
            }
            else
            {
                // 인벤토리 내 빈 공간 - 원래 슬롯으로 되돌리기
                Debug.Log($"[DROP] 인벤토리 내 빈 공간 - 원래 슬롯으로 되돌리기");
                if (originalSlot != null)
                {
                    originalSlot.item = liftedItem;
                    originalSlot.UpdateSlotUI();
                    CheckAndSyncHotbar(originalSlot);
                }
                shouldDropToWorld = false;
            }
        }

        // 월드에 드롭
        if (shouldDropToWorld)
        {
            Debug.Log($"[DROP] 월드에 드롭 실행!");

            bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            int dropAmount = isShiftPressed && liftedItem.isStackable ?
                Mathf.Max(1, liftedItem.quantity / 2) : liftedItem.quantity;

            // 아이템 복사본 생성하여 드롭
            Item itemCopy = Instantiate(liftedItem);
            itemCopy.quantity = dropAmount;
            DropItemToWorld(itemCopy);

            // 들고 있는 아이템을 완전히 제거 (복사 방지)
            liftedItem = null;
            if (originalSlot != null)
            {
                originalSlot.item = null;
                originalSlot.UpdateSlotUI();
                CheckAndSyncHotbar(originalSlot);
            }
            originalSlot = null;
        }

        // 상태 초기화
        UpdateCursorUI();
        UpdateDropZoneIndicator(false);
    }

    public void SetHoveredSlot(ItemSlot slot)
    {
        hoveredSlot = slot;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public void OnSlotClicked(ItemSlot clickedSlot)
    {
        ItemSlot prevOriginalSlot = originalSlot;

        if (liftedItem == null)
        {
            // 슬롯에서 아이템을 들어올리기
            if (clickedSlot.item != null)
            {
                liftedItem = clickedSlot.item;
                originalSlot = clickedSlot;
                clickedSlot.item = null;
                clickedSlot.UpdateSlotUI();
                Debug.Log($"[LIFT] 아이템 들어올림: {liftedItem.itemName} x{liftedItem.quantity} (인스턴스 ID: {liftedItem.GetInstanceID()})");
            }
        }
        else
        {
            // 이미 들고 있는 아이템이 있는 경우
            if (clickedSlot.item != null)
            {
                // 슬롯에 아이템이 있으면 서로 교체
                Item itemInNewSlot = clickedSlot.item;
                clickedSlot.item = liftedItem;
                clickedSlot.UpdateSlotUI();

                Debug.Log($"[SWAP] 아이템 교체 - 놓은 아이템: {liftedItem.itemName} (ID: {liftedItem.GetInstanceID()}), 들은 아이템: {itemInNewSlot.itemName} (ID: {itemInNewSlot.GetInstanceID()})");

                liftedItem = itemInNewSlot;
                originalSlot = clickedSlot;
            }
            else
            {
                // 빈 슬롯에 아이템 놓기
                clickedSlot.item = liftedItem;
                clickedSlot.UpdateSlotUI();

                Debug.Log($"[PLACE] 아이템 배치 완료: {liftedItem.itemName} (ID: {liftedItem.GetInstanceID()})");

                liftedItem = null;
                originalSlot = null;
            }
        }

        UpdateCursorUI();
        CheckAndSyncHotbar(clickedSlot);

        if (prevOriginalSlot != null && prevOriginalSlot != clickedSlot)
            CheckAndSyncHotbar(prevOriginalSlot);
    }

    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd == null) return false;

        // 아이템 인스턴스 생성 (원본 보호)
        Item itemCopy = Instantiate(itemToAdd);

        if (itemCopy.isStackable)
        {
            // 스택 가능한 아이템의 경우 기존 아이템과 합치기 시도
            foreach (ItemSlot slot in allInventorySlots)
            {
                if (slot.item != null && slot.item.itemName == itemCopy.itemName)
                {
                    slot.item.quantity += itemCopy.quantity;
                    slot.UpdateSlotUI();
                    CheckAndSyncHotbar(slot);
                    Debug.Log($"{itemCopy.itemName} x{itemCopy.quantity} 기존 스택에 추가됨");
                    return true;
                }
            }
        }

        // 빈 슬롯 찾아서 아이템 배치
        foreach (ItemSlot slot in allInventorySlots)
        {
            if (slot.item == null)
            {
                slot.item = itemCopy;
                slot.UpdateSlotUI();
                CheckAndSyncHotbar(slot);
                Debug.Log($"{itemCopy.itemName} x{itemCopy.quantity} 새 슬롯에 추가됨");
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다!");
        DropItemToWorld(itemCopy);
        return false;
    }

    private void CheckAndSyncHotbar(ItemSlot changedSlot)
    {
        if (changedSlot == null || inventoryHotbarGrid == null ||
            !changedSlot.transform.IsChildOf(inventoryHotbarGrid))
            return;

        // Hotbar 컴포넌트 찾기
        Hotbar mainHotbar = FindObjectOfType<Hotbar>();
        if (mainHotbar == null)
        {
            Debug.LogWarning("CheckAndSyncHotbar: Hotbar 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        int slotIndex = changedSlot.transform.GetSiblingIndex();
        Debug.Log($"CheckAndSyncHotbar 호출: 슬롯 {slotIndex}, 아이템: {changedSlot.item?.itemName}");
        mainHotbar.UpdateSlot(slotIndex, changedSlot.item);
    }

    private void SyncHotbarToInventory()
    {
        // 주석 처리 - Hotbar 참조 제거
        /*
        if (mainHotbar == null || inventoryHotbarGrid == null) return;

        for (int i = 0; i < mainHotbar.hotbarSlots.Length; i++)
        {
            ItemSlot inventorySlot = inventoryHotbarGrid.GetChild(i)?.GetComponent<ItemSlot>();
            if (inventorySlot != null)
                inventorySlot.SetItem(mainHotbar.hotbarSlots[i].item);
        }
        */
    }

    private void UpdateCursorUI()
    {
        if (cursorItemObject == null) return;

        if (liftedItem != null)
        {
            cursorItemObject.SetActive(true);
            if (cursorItemIcon != null) cursorItemIcon.sprite = liftedItem.icon;
            if (cursorQuantityText != null)
            {
                cursorQuantityText.enabled = liftedItem.isStackable && liftedItem.quantity > 1;
                cursorQuantityText.text = liftedItem.quantity.ToString();
            }
        }
        else
        {
            cursorItemObject.SetActive(false);
        }
    }

    private void CancelLift()
    {
        if (originalSlot == null) return;

        originalSlot.item = liftedItem;
        originalSlot.UpdateSlotUI();
        CheckAndSyncHotbar(originalSlot);
        liftedItem = null;
        originalSlot = null;
        UpdateCursorUI();
    }

    private void UpdateDropZoneIndicator(bool show)
    {
        if (dropZoneUIObject == null) return;

        dropZoneUIObject.SetActive(show);

        if (show && liftedItem != null)
        {
            // 드롭 존 아이콘 설정
            if (dropZoneIcon != null && liftedItem.icon != null)
            {
                dropZoneIcon.sprite = liftedItem.icon;
                dropZoneIcon.gameObject.SetActive(true);
            }

            // 드롭 존 배경 색상 설정
            if (dropZoneBackground != null)
            {
                bool canDrop = fieldItemPrefab != null && playerMovement != null;
                dropZoneBackground.color = canDrop ?
                    new Color(0f, 1f, 0f, 0.3f) : // 초록색 (드롭 가능)
                    new Color(1f, 0f, 0f, 0.3f);  // 빨간색 (드롭 불가)
            }

            // 드롭 존 텍스트 설정
            if (dropZoneText != null)
            {
                bool canDrop = fieldItemPrefab != null && playerMovement != null;
                dropZoneText.text = canDrop ?
                    "아이템을 여기에 놓으세요\n(Shift + 드래그: 절반만 드롭)" :
                    "여기에 드롭할 수 없습니다";
                dropZoneText.color = canDrop ? Color.white : Color.red;
            }
        }
    }

    private void DropItemToWorld(Item item)
    {
        if (fieldItemPrefab == null || playerMovement == null || item == null) return;

        Debug.Log($"DropItemToWorld 호출됨: {item.itemName} x{item.quantity}");

        // 플레이어 주변 랜덤 위치에 드롭
        Vector3 playerPos = playerMovement.transform.position;
        Vector2 randomOffset = Random.insideUnitCircle * 1.5f;
        Vector3 dropPosition = playerPos + new Vector3(randomOffset.x, randomOffset.y, 0);

        GameObject itemObject = Instantiate(fieldItemPrefab, dropPosition, Quaternion.identity);
        if (itemObject == null)
        {
            Debug.LogError("fieldItemPrefab 인스턴스 생성 실패!");
            return;
        }

        // FieldItem 컴포넌트 확인 및 설정
        FieldItem fieldItem = itemObject.GetComponent<FieldItem>();
        if (fieldItem != null)
        {
            fieldItem.SetItem(item);
            Debug.Log($"FieldItem에 아이템 설정 완료: {item.itemName}");
        }
        else
        {
            Debug.LogError("FieldItem 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        // 필요한 컴포넌트들이 있는지 확인하고 없으면 추가
        if (itemObject.GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = itemObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
            Debug.Log("CircleCollider2D 자동 추가됨");
        }

        if (itemObject.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = itemObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.drag = 5f;
            Debug.Log("Rigidbody2D 자동 추가됨");
        }

        // 물리 효과 추가
        Rigidbody2D rigidbody = itemObject.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            Vector2 throwForce = randomOffset.normalized * Random.Range(1f, 3f);
            rigidbody.AddForce(throwForce, ForceMode2D.Impulse);
            Debug.Log($"아이템에 물리 효과 적용: {throwForce}");
        }

        // 컴포넌트 상태 로깅
        Collider2D col = itemObject.GetComponent<Collider2D>();
        Debug.Log($"드롭된 아이템 컴포넌트 상태:");
        Debug.Log($"- FieldItem: {fieldItem != null}");
        Debug.Log($"- Collider2D: {col != null}, isTrigger: {col?.isTrigger}");
        Debug.Log($"- Rigidbody2D: {rigidbody != null}");
        Debug.Log($"- 위치: {dropPosition}");
    }

    private void OnGUI()
    {
        if (!isOpen || liftedItem == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool isOverInventoryPanel = IsMouseOverInventoryPanel();

        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 150));
        GUILayout.Label($"🖱️ 마우스: {mousePos}");
        GUILayout.Label($"📦 인벤토리 패널 위: {isOverInventoryPanel}");
        GUILayout.Label($"🎒 들고있는 아이템: {liftedItem.itemName}");
        GUILayout.Label($"📊 수량: {liftedItem.quantity}");
        GUILayout.Label($"🔍 호버된 슬롯: {(hoveredSlot != null ? hoveredSlot.name : "없음")}");

        if (isOverInventoryPanel)
        {
            if (hoveredSlot != null)
                GUILayout.Label("✅ 슬롯에 배치 가능");
            else
                GUILayout.Label("🔄 원래 슬롯으로 되돌림");
        }
        else
        {
            GUILayout.Label("🌍 월드에 드롭!");
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// 특정 슬롯에서 아이템을 제거하고 월드에 드롭합니다.
    /// </summary>
    /// <param name="slot">대상 슬롯</param>
    /// <param name="amount">드롭할 개수</param>
    public void DropItemFromSlot(ItemSlot slot, int amount = 1)
    {
        if (slot == null || slot.item == null || amount <= 0) return;

        // 드롭할 아이템 생성 (ScriptableObject를 복제)
        Item dropItem = ScriptableObject.CreateInstance<Item>();
        dropItem.itemName = slot.item.itemName;
        dropItem.icon = slot.item.icon;
        dropItem.itemDescription = slot.item.itemDescription;
        dropItem.itemType = slot.item.itemType;
        dropItem.isStackable = slot.item.isStackable;
        dropItem.quantity = Mathf.Min(amount, slot.item.quantity);

        // 슬롯에서 아이템 제거
        slot.item.quantity -= dropItem.quantity;
        if (slot.item.quantity <= 0)
        {
            slot.item = null;
        }
        slot.UpdateSlotUI();

        // 월드에 드롭
        DropItemToWorld(dropItem);

        // 핫바 동기화
        CheckAndSyncHotbar(slot);
    }

    // 테스트용 아이템 추가 (디버깅/테스트 목적)
    [ContextMenu("Add Test Items")]
    private void AddTestItems()
    {
        Debug.Log("[TEST] 테스트 아이템 추가 중...");

        // 첫 번째 빈 슬롯들에 테스트 아이템 추가
        // 실제 게임에서는 이 메서드를 호출하지 않거나 제거하세요

        // 예시: 만약 Item ScriptableObject들이 있다면
        // 아래 코드의 주석을 해제하고 실제 아이템으로 교체하세요
        /*
        if (allInventorySlots.Count > 0 && allInventorySlots[0].item == null)
        {
            // 예시 아이템 생성 (실제로는 ScriptableObject 에셋 참조 사용)
            Item testItem = ScriptableObject.CreateInstance<Item>();
            testItem.itemName = "테스트 검";
            testItem.quantity = 1;
            allInventorySlots[0].SetItem(testItem);
        }
        */

        Debug.Log("[TEST] 테스트 아이템 추가 완료");
    }

    // Public properties for external access
    public Transform InventoryHotbarGrid => inventoryHotbarGrid;

    public void CloseInventory()
    {
        if (inventoryUIPanel == null || playerMovement == null || !isOpen) return;

        isOpen = false;
        inventoryUIPanel.SetActive(false);
        playerMovement.EnableGameplayInput();

        if (liftedItem != null)
        {
            CancelLift();
        }

        Debug.Log("인벤토리 닫힘 (메뉴에서 호출)");
    }
}
