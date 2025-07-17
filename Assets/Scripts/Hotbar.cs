using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Hotbar : MonoBehaviour
{
    [SerializeField] public ItemSlot[] hotbarSlots;
    [SerializeField] private GameObject selectionBox;
    [SerializeField] private Transform inventoryHotbarGrid; // 인벤토리 내 핫바 그리드 참조

    private int selectedSlotIndex = 0;
    private PlayerControls playerControls;
    private InventoryManager inventory;
    private ItemSlot[] inventoryHotbarSlots; // 인벤토리 내 핫바 슬롯들

    private void Start()
    {
        Debug.Log("=== Hotbar Start 시작 ===");

        // 강제 초기화 수행
        ForceInitializeHotbar();

        inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("InventoryManager.Instance가 null입니다!");
            return;
        }
        Debug.Log("InventoryManager 참조 성공");

        // 인벤토리 핫바 슬롯들 찾기
        FindInventoryHotbarSlots();

        StartCoroutine(InitializeHotbar());

        // 초기 동기화 (잠시 후에 수행)
        StartCoroutine(InitialSync());

        // 주기적으로 동기화 확인
        StartCoroutine(SyncWithInventoryHotbar());

        Debug.Log("=== Hotbar Start 완료 ===");
    }

    private IEnumerator InitialSync()
    {
        yield return new WaitForSeconds(0.5f); // 모든 초기화가 완료될 때까지 대기

        Debug.Log("초기 핫바 동기화 시작");
        LogHotbarState();

        if (inventoryHotbarSlots != null && hotbarSlots != null)
        {
            for (int i = 0; i < Mathf.Min(inventoryHotbarSlots.Length, hotbarSlots.Length); i++)
            {
                if (inventoryHotbarSlots[i] != null && inventoryHotbarSlots[i].item != null)
                {
                    Debug.Log($"초기 동기화: 슬롯 {i}에 {inventoryHotbarSlots[i].item.itemName} 적용");
                    UpdateSlot(i, inventoryHotbarSlots[i].item);
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        Debug.Log("초기 동기화 완료 후 상태:");
        LogHotbarState();
    }

    private void LogHotbarState()
    {
        Debug.Log("=== 핫바 상태 로깅 ===");
        if (hotbarSlots != null)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                Debug.Log($"메인 핫바 슬롯 {i}: {hotbarSlots[i]?.item?.itemName}");
            }
        }

        if (inventoryHotbarSlots != null)
        {
            for (int i = 0; i < inventoryHotbarSlots.Length; i++)
            {
                Debug.Log($"인벤토리 핫바 슬롯 {i}: {inventoryHotbarSlots[i]?.item?.itemName}");
            }
        }
        Debug.Log("==================");
    }

    private void FindInventoryHotbarSlots()
    {
        Debug.Log("FindInventoryHotbarSlots 시작");

        if (inventoryHotbarGrid == null && inventory != null)
        {
            // InventoryManager의 public 프로퍼티를 통해 inventoryHotbarGrid 가져오기
            inventoryHotbarGrid = inventory.InventoryHotbarGrid;
            Debug.Log($"InventoryHotbarGrid 참조 획득: {inventoryHotbarGrid != null}");

            if (inventoryHotbarGrid != null)
            {
                Debug.Log($"InventoryHotbarGrid 이름: {inventoryHotbarGrid.name}");
                Debug.Log($"InventoryHotbarGrid 위치: {inventoryHotbarGrid.transform.position}");
            }
        }

        // 여전히 null이면 다른 방법으로 찾아보기
        if (inventoryHotbarGrid == null)
        {
            Debug.LogWarning("InventoryHotbarGrid가 여전히 null입니다. 다른 방법으로 찾는 중...");

            // GameObject 이름으로 찾기
            GameObject hotbarGridObj = GameObject.Find("InventoryHotbarGrid");
            if (hotbarGridObj == null)
                hotbarGridObj = GameObject.Find("HotbarGrid");
            if (hotbarGridObj == null)
                hotbarGridObj = GameObject.Find("Hotbar");

            if (hotbarGridObj != null)
            {
                inventoryHotbarGrid = hotbarGridObj.transform;
                Debug.Log($"GameObject.Find로 찾음: {inventoryHotbarGrid.name}");
            }
        }

        if (inventoryHotbarGrid != null)
        {
            inventoryHotbarSlots = inventoryHotbarGrid.GetComponentsInChildren<ItemSlot>();
            Debug.Log($"인벤토리 핫바 슬롯 {inventoryHotbarSlots.Length}개 발견");

            // 각 슬롯 상태 로깅
            for (int i = 0; i < inventoryHotbarSlots.Length; i++)
            {
                Debug.Log($"인벤토리 핫바 슬롯 {i}: {inventoryHotbarSlots[i]?.name}, 아이템: {inventoryHotbarSlots[i]?.item?.itemName}");
            }
        }
        else
        {
            Debug.LogError("inventoryHotbarGrid를 찾을 수 없습니다! Unity Inspector에서 InventoryManager의 inventoryHotbarGrid를 설정해주세요.");
        }
    }

    private IEnumerator SyncWithInventoryHotbar()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 0.1초마다 체크

            if (inventoryHotbarSlots != null && hotbarSlots != null)
            {
                for (int i = 0; i < Mathf.Min(inventoryHotbarSlots.Length, hotbarSlots.Length); i++)
                {
                    if (inventoryHotbarSlots[i] != null && hotbarSlots[i] != null)
                    {
                        // 인벤토리 핫바 슬롯과 메인 핫바 슬롯이 다르면 동기화
                        if (!AreItemsEqual(inventoryHotbarSlots[i].item, hotbarSlots[i].item))
                        {
                            Debug.Log($"동기화 필요 감지 - 슬롯 {i}:");
                            Debug.Log($"  인벤토리: {inventoryHotbarSlots[i].item?.itemName} x{inventoryHotbarSlots[i].item?.quantity}");
                            Debug.Log($"  핫바: {hotbarSlots[i].item?.itemName} x{hotbarSlots[i].item?.quantity}");

                            UpdateSlot(i, inventoryHotbarSlots[i].item);
                            Debug.Log($"핫바 슬롯 {i} 동기화 완료: {inventoryHotbarSlots[i].item?.itemName}");
                        }
                    }
                }
            }
            else
            {
                if (inventoryHotbarSlots == null) Debug.LogWarning("inventoryHotbarSlots가 null입니다!");
                if (hotbarSlots == null) Debug.LogWarning("hotbarSlots가 null입니다!");
            }
        }
    }

    private bool AreItemsEqual(Item item1, Item item2)
    {
        if (item1 == null && item2 == null) return true;
        if (item1 == null || item2 == null) return false;

        return item1.itemName == item2.itemName && item1.quantity == item2.quantity;
    }

    private void Awake()
    {
        playerControls = PlayerControlsManager.playerControls;
    }
    private void OnEnable()
    {
        if (playerControls == null) return;
        playerControls.UI.Enable();
        playerControls.UI.Hotbar1.performed += SelectSlot_0; playerControls.UI.Hotbar2.performed += SelectSlot_1; playerControls.UI.Hotbar3.performed += SelectSlot_2;
        playerControls.UI.Hotbar4.performed += SelectSlot_3; playerControls.UI.Hotbar5.performed += SelectSlot_4; playerControls.UI.Hotbar6.performed += SelectSlot_5;
        playerControls.UI.DropItem.performed += OnDropItemKeyPressed;
    }
    private void OnDisable()
    {
        if (playerControls == null) return;
        playerControls.UI.Disable();
        playerControls.UI.Hotbar1.performed -= SelectSlot_0; playerControls.UI.Hotbar2.performed -= SelectSlot_1; playerControls.UI.Hotbar3.performed -= SelectSlot_2;
        playerControls.UI.Hotbar4.performed -= SelectSlot_3; playerControls.UI.Hotbar5.performed -= SelectSlot_4; playerControls.UI.Hotbar6.performed -= SelectSlot_5;
        playerControls.UI.DropItem.performed -= OnDropItemKeyPressed;
    }
    private void SelectSlot_0(InputAction.CallbackContext ctx) => SelectSlot(0); private void SelectSlot_1(InputAction.CallbackContext ctx) => SelectSlot(1); private void SelectSlot_2(InputAction.CallbackContext ctx) => SelectSlot(2); private void SelectSlot_3(InputAction.CallbackContext ctx) => SelectSlot(3); private void SelectSlot_4(InputAction.CallbackContext ctx) => SelectSlot(4); private void SelectSlot_5(InputAction.CallbackContext ctx) => SelectSlot(5);
    private void Update()
    {
        // 인벤토리가 열려있으면 휠 스크롤 비활성화
        if (inventory != null && inventory.IsOpen())
        {
            return;
        }

        // 마우스 휠 스크롤로 핫바 슬롯 선택
        Vector2 scrollInput = Mouse.current.scroll.ReadValue();

        if (scrollInput.y != 0)
        {
            int direction = scrollInput.y > 0 ? -1 : 1; // 위로: -1 (왼쪽), 아래로: 1 (오른쪽)

            // 위로 스크롤: 왼쪽으로 이동 (-1)
            // 아래로 스크롤: 오른쪽으로 이동 (+1)
            int newIndex = selectedSlotIndex + direction;

            // 순환 처리 (0~5 사이)
            if (newIndex >= hotbarSlots.Length)
                newIndex = 0;
            else if (newIndex < 0)
                newIndex = hotbarSlots.Length - 1;

            SelectSlot(newIndex);
            Debug.Log($"마우스 휠 스크롤: {(direction < 0 ? "위로(왼쪽)" : "아래로(오른쪽)")}, 선택된 슬롯: {newIndex}");
        }
    }

    private IEnumerator InitializeHotbar() { yield return new WaitForEndOfFrame(); SelectSlot(0); }
    private void OnDropItemKeyPressed(InputAction.CallbackContext context)
    {
        if (inventory != null && !inventory.IsOpen())
        {
            ItemSlot selectedSlot = hotbarSlots[selectedSlotIndex];
            if (selectedSlot.item != null)
            {
                // 안전한 슬롯 드롭 메서드 사용
                inventory.DropItemFromSlot(selectedSlot, 1);
            }
        }
    }
    void SelectSlot(int index) { if (hotbarSlots == null || index < 0 || index >= hotbarSlots.Length) return; selectedSlotIndex = index; if (selectionBox != null && hotbarSlots[index] != null) { selectionBox.SetActive(true); selectionBox.transform.position = hotbarSlots[index].transform.position; } }
    public void UpdateSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length)
        {
            Debug.LogWarning($"UpdateSlot: 잘못된 슬롯 인덱스 {slotIndex}");
            return;
        }

        Debug.Log($"UpdateSlot 호출: 슬롯 {slotIndex}, 아이템: {item?.itemName}");

        // 이미 인스턴스인 아이템은 직접 할당, 원본은 SetItem 사용
        if (item == null)
        {
            hotbarSlots[slotIndex].item = null;
            Debug.Log($"슬롯 {slotIndex}을 null로 설정");
        }
        else if (item.name.Contains("(Clone)"))
        {
            // 이미 인스턴스인 경우 직접 할당
            hotbarSlots[slotIndex].item = item;
            Debug.Log($"슬롯 {slotIndex}에 Clone 아이템 직접 할당: {item.itemName}");
        }
        else
        {
            // 원본인 경우 SetItem으로 인스턴스 생성
            hotbarSlots[slotIndex].SetItem(item);
            Debug.Log($"슬롯 {slotIndex}에 원본 아이템으로 SetItem 호출: {item.itemName}");
            return; // SetItem에서 UpdateSlotUI 호출됨
        }

        hotbarSlots[slotIndex].UpdateSlotUI();
        Debug.Log($"슬롯 {slotIndex} UI 업데이트 완료");

        // 인벤토리 핫바에도 동기화 (무한 루프 방지 로직 포함)
        SyncSlotToInventory(slotIndex);
    }

    private void SyncSlotToInventory(int slotIndex)
    {
        if (inventoryHotbarSlots != null && slotIndex < inventoryHotbarSlots.Length)
        {
            if (inventoryHotbarSlots[slotIndex] != null)
            {
                // 무한 루프 방지: 이미 같은 아이템이면 동기화하지 않음
                if (!AreItemsEqual(inventoryHotbarSlots[slotIndex].item, hotbarSlots[slotIndex].item))
                {
                    // 핫바 슬롯을 인벤토리 핫바 슬롯에 동기화
                    inventoryHotbarSlots[slotIndex].item = hotbarSlots[slotIndex].item;
                    inventoryHotbarSlots[slotIndex].UpdateSlotUI();
                    Debug.Log($"인벤토리 핫바 슬롯 {slotIndex} 동기화됨: {hotbarSlots[slotIndex].item?.itemName}");
                }
            }
        }
    }

    public void SyncAllSlotsToInventory()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            SyncSlotToInventory(i);
        }
    }

    // Unity Inspector에서 설정이 안되어 있을 때를 위한 강제 초기화
    [ContextMenu("Force Initialize Hotbar")]
    public void ForceInitializeHotbar()
    {
        Debug.Log("=== ForceInitializeHotbar 시작 ===");

        // 1. hotbarSlots 배열이 비어있으면 자동으로 찾기
        if (hotbarSlots == null || hotbarSlots.Length == 0)
        {
            hotbarSlots = GetComponentsInChildren<ItemSlot>();
            Debug.Log($"hotbarSlots 자동 할당: {hotbarSlots.Length}개");
        }

        // 2. selectionBox가 없으면 찾기
        if (selectionBox == null)
        {
            Transform selectionBoxTransform = transform.Find("SelectionBox");
            if (selectionBoxTransform == null)
                selectionBoxTransform = transform.Find("Selection");
            if (selectionBoxTransform == null)
                selectionBoxTransform = transform.Find("Highlight");

            if (selectionBoxTransform != null)
            {
                selectionBox = selectionBoxTransform.gameObject;
                Debug.Log($"selectionBox 자동 할당: {selectionBox.name}");
            }
        }

        // 3. 인벤토리와 연결
        if (inventory == null)
        {
            inventory = InventoryManager.Instance;
            Debug.Log($"inventory 자동 할당: {inventory != null}");
        }

        // 4. 인벤토리 핫바 그리드 찾기
        FindInventoryHotbarSlots();

        Debug.Log("=== ForceInitializeHotbar 완료 ===");
    }
}