using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // 코루틴을 위해 추가

public class Hotbar : MonoBehaviour
{
    [Tooltip("실제 게임 화면에 항상 보이는 핫바 슬롯들. Inspector에서 연결해야 합니다.")]
    [SerializeField] public ItemSlot[] hotbarSlots;

    [Tooltip("선택된 슬롯을 표시하는 UI 이미지. Inspector에서 연결해야 합니다.")]
    [SerializeField] private GameObject selectionBox;

    private int selectedSlotIndex = 0;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.UI.Hotbar1.performed += context => SelectSlot(0);
        playerControls.UI.Hotbar2.performed += context => SelectSlot(1);
        playerControls.UI.Hotbar3.performed += context => SelectSlot(2);
        playerControls.UI.Hotbar4.performed += context => SelectSlot(3);
        playerControls.UI.Hotbar5.performed += context => SelectSlot(4);
        playerControls.UI.Hotbar6.performed += context => SelectSlot(5);
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        if (hotbarSlots == null || hotbarSlots.Length == 0) Debug.LogError("Hotbar Slots가 Inspector에 연결되지 않았습니다!");
        if (selectionBox == null) Debug.LogError("Selection Box가 Inspector에 연결되지 않았습니다!");

        // 코루틴을 시작하여 모든 레이아웃 계산이 끝난 후 초기화 실행
        StartCoroutine(InitializeSelectionBox());
    }

    private IEnumerator InitializeSelectionBox()
    {
        // 모든 Update와 UI 렌더링이 끝나는 프레임의 마지막 시점까지 대기
        yield return new WaitForEndOfFrame();

        // 이제 슬롯들의 위치가 100% 확정된 상태이므로, 첫 번째 슬롯 선택
        SelectSlot(0);
    }

    void SelectSlot(int index)
    {
        if (hotbarSlots == null || index < 0 || index >= hotbarSlots.Length || hotbarSlots[index] == null) return;
        selectedSlotIndex = index;

        if (selectionBox != null)
        {
            selectionBox.SetActive(true);
            selectionBox.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
        }
    }

    public void UpdateSlot(int slotIndex, Item item)
    {
        if (slotIndex < 0 || slotIndex >= hotbarSlots.Length) return;
        hotbarSlots[slotIndex].SetItem(item);
    }
}