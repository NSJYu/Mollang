using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Hotbar : MonoBehaviour
{
    [SerializeField] public ItemSlot[] hotbarSlots;
    [SerializeField] private GameObject selectionBox;
    private int selectedSlotIndex = 0;
    private PlayerControls playerControls;
    private Inventory inventory;
    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
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
    private IEnumerator Start() { yield return new WaitForEndOfFrame(); SelectSlot(0); }
    private void OnDropItemKeyPressed(InputAction.CallbackContext context) { if (inventory != null && !inventory.IsOpen()) { ItemSlot selectedSlot = hotbarSlots[selectedSlotIndex]; if (selectedSlot.item != null) { inventory.DropItem(selectedSlot.item, 1); if (selectedSlot.item.quantity <= 0) selectedSlot.SetItem(null); else selectedSlot.UpdateSlotUI(); } } }
    void SelectSlot(int index) { if (hotbarSlots == null || index < 0 || index >= hotbarSlots.Length) return; selectedSlotIndex = index; if (selectionBox != null && hotbarSlots[index] != null) { selectionBox.SetActive(true); selectionBox.transform.position = hotbarSlots[index].transform.position; } }
    public void UpdateSlot(int slotIndex, Item item) { if (slotIndex < 0 || slotIndex >= hotbarSlots.Length) return; hotbarSlots[slotIndex].SetItem(item); }
}