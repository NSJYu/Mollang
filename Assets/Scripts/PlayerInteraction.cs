using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactionPopup;
    private FieldItem nearbyItem;
    private Inventory inventory;
    private PlayerControls playerControls;
    private void Awake() { inventory = FindObjectOfType<Inventory>(); playerControls = PlayerControlsManager.playerControls; }
    private void OnEnable() { if (playerControls != null) { playerControls.UI.Enable(); playerControls.UI.Interact.performed += OnInteract; } }
    private void OnDisable() { if (playerControls != null) { playerControls.UI.Disable(); playerControls.UI.Interact.performed -= OnInteract; } }
    private void Start() { if (inventory == null) Debug.LogError("씬에 Inventory 스크립트가 없습니다!"); if (interactionPopup != null) interactionPopup.SetActive(false); }
    private void OnInteract(InputAction.CallbackContext context) { if (nearbyItem != null && inventory != null) { if (inventory.AddItem(nearbyItem.GetItem())) { nearbyItem = null; if (interactionPopup != null) interactionPopup.SetActive(false); } } }
    private void OnTriggerEnter2D(Collider2D other) { if (other.TryGetComponent<FieldItem>(out var fieldItem)) { nearbyItem = fieldItem; if (interactionPopup != null) interactionPopup.SetActive(true); } }
    private void OnTriggerExit2D(Collider2D other) { if (other.TryGetComponent<FieldItem>(out var fieldItem) && fieldItem == nearbyItem) { nearbyItem = null; if (interactionPopup != null) interactionPopup.SetActive(false); } }
}