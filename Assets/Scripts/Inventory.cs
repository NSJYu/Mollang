using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    [Header("UI 설정 (직접 연결)")]
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private GameObject cursorItemObject;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Transform inventoryHotbarGrid;
    [Header("플레이어 및 아이템 설정")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject fieldItemPrefab;
    private Image cursorItemIcon; private TextMeshProUGUI cursorQuantityText; private bool isOpen = false; private Item liftedItem; private ItemSlot originalSlot; private Hotbar mainHotbar; private ItemSlot hoveredSlot; private List<ItemSlot> allInventorySlots = new List<ItemSlot>(); private PlayerControls playerControls;
    private void Awake() { playerControls = PlayerControlsManager.playerControls; if (cursorItemObject != null) { cursorItemIcon = cursorItemObject.GetComponent<Image>(); cursorQuantityText = cursorItemObject.GetComponentInChildren<TextMeshProUGUI>(); } if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>(); if(inventoryGrid != null) inventoryGrid.GetComponentsInChildren<ItemSlot>(true, allInventorySlots); if(inventoryHotbarGrid != null) inventoryHotbarGrid.GetComponentsInChildren<ItemSlot>(true, allInventorySlots); }
    private void OnEnable() { if(playerControls == null) return; playerControls.UI.Enable(); playerControls.UI.OpenInventory.performed += ToggleInventory; playerControls.UI.DropItem.performed += OnDropItemKeyPressed; }
    private void OnDisable() { if(playerControls == null) return; playerControls.UI.Disable(); playerControls.UI.OpenInventory.performed -= ToggleInventory; playerControls.UI.DropItem.performed -= OnDropItemKeyPressed; }
    private void Start() { if (inventoryUIPanel != null) inventoryUIPanel.SetActive(false); if (cursorItemObject != null) cursorItemObject.SetActive(false); mainHotbar = FindObjectOfType<Hotbar>(); }
    public void ToggleInventory(InputAction.CallbackContext context) { if (inventoryUIPanel == null || playerMovement == null) return; isOpen = !isOpen; inventoryUIPanel.SetActive(isOpen); if (isOpen) { playerMovement.DisableGameplayInput(); SyncHotbarToInventory(); } else { playerMovement.EnableGameplayInput(); if (liftedItem != null) CancelLift(); } }
    private void OnDropItemKeyPressed(InputAction.CallbackContext context) { if (isOpen && EventSystem.current.IsPointerOverGameObject() && hoveredSlot != null && hoveredSlot.item != null) { DropItem(hoveredSlot.item, 1); if (hoveredSlot.item.quantity <= 0) hoveredSlot.SetItem(null); else hoveredSlot.UpdateSlotUI(); CheckAndSyncHotbar(hoveredSlot); } }
    private void Update() { if (isOpen && liftedItem != null && cursorItemObject != null) { (cursorItemObject.transform as RectTransform).position = Mouse.current.position.ReadValue(); } if (Mouse.current.leftButton.wasPressedThisFrame) { if (isOpen) { if (EventSystem.current.IsPointerOverGameObject()) { if (hoveredSlot != null) OnSlotClicked(hoveredSlot); } else { if (liftedItem != null) { DropItem(liftedItem, liftedItem.quantity); liftedItem = null; originalSlot = null; UpdateCursorUI(); } } } } }
    public void SetHoveredSlot(ItemSlot slot) { hoveredSlot = slot; }
    public bool IsOpen() { return isOpen; }
    public void OnSlotClicked(ItemSlot clickedSlot) { ItemSlot prevOriginalSlot = originalSlot; if (liftedItem == null) { if (clickedSlot.item != null) { liftedItem = clickedSlot.item; originalSlot = clickedSlot; clickedSlot.SetItem(null); } } else { if (clickedSlot.item != null) { Item itemInNewSlot = clickedSlot.item; clickedSlot.SetItem(liftedItem); liftedItem = itemInNewSlot; originalSlot = clickedSlot; } else { clickedSlot.SetItem(liftedItem); liftedItem = null; originalSlot = null; } } UpdateCursorUI(); CheckAndSyncHotbar(clickedSlot); if (prevOriginalSlot != null && prevOriginalSlot != clickedSlot) CheckAndSyncHotbar(prevOriginalSlot); }
    public void DropItem(Item item, int amount) { if (fieldItemPrefab == null || playerMovement == null || item == null) return; int dropAmount = Mathf.Min(amount, item.quantity); if (dropAmount <= 0) return; Item itemToInstantiate = Instantiate(item); itemToInstantiate.quantity = dropAmount; item.quantity -= dropAmount; Vector3 dropPosition = playerMovement.transform.position; GameObject itemObject = Instantiate(fieldItemPrefab, dropPosition, Quaternion.identity); itemObject.GetComponent<FieldItem>().SetItem(itemToInstantiate); }
    public bool AddItem(Item itemToAdd) { if (itemToAdd.isStackable) { foreach (ItemSlot slot in allInventorySlots) { if (slot.item != null && slot.item.itemName == itemToAdd.itemName) { slot.item.quantity += itemToAdd.quantity; slot.UpdateSlotUI(); CheckAndSyncHotbar(slot); return true; } } } foreach (ItemSlot slot in allInventorySlots) { if (slot.item == null) { slot.SetItem(itemToAdd); CheckAndSyncHotbar(slot); return true; } } Debug.Log("인벤토리가 가득 찼습니다!"); DropItem(itemToAdd, itemToAdd.quantity); return false; }
    private void CheckAndSyncHotbar(ItemSlot changedSlot) { if (changedSlot == null || inventoryHotbarGrid == null || mainHotbar == null || !changedSlot.transform.IsChildOf(inventoryHotbarGrid)) return; int slotIndex = changedSlot.transform.GetSiblingIndex(); mainHotbar.UpdateSlot(slotIndex, changedSlot.item); }
    private void SyncHotbarToInventory() { if (mainHotbar == null || inventoryHotbarGrid == null) return; for (int i = 0; i < mainHotbar.hotbarSlots.Length; i++) { ItemSlot inventorySlot = inventoryHotbarGrid.GetChild(i)?.GetComponent<ItemSlot>(); if (inventorySlot != null) inventorySlot.SetItem(mainHotbar.hotbarSlots[i].item); } }
    private void UpdateCursorUI() { if (cursorItemObject == null) return; if (liftedItem != null) { cursorItemObject.SetActive(true); cursorItemIcon.sprite = liftedItem.icon; cursorQuantityText.enabled = liftedItem.isStackable && liftedItem.quantity > 1; cursorQuantityText.text = liftedItem.quantity.ToString(); } else { cursorItemObject.SetActive(false); } }
    private void CancelLift() { if (originalSlot == null) return; originalSlot.SetItem(liftedItem); CheckAndSyncHotbar(originalSlot); liftedItem = null; originalSlot = null; UpdateCursorUI(); }
}