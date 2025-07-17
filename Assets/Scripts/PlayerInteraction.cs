using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject interactionPopup;
    [SerializeField] private float interactionRadius = 1.5f; // 상호작용 감지 반경

    private FieldItem nearbyItem;
    private InventoryManager inventory;
    private PlayerControls playerControls;
    private CircleCollider2D interactionTrigger;

    private void Awake()
    {
        playerControls = PlayerControlsManager.playerControls;

        // 상호작용 전용 트리거 콜라이더 생성
        CreateInteractionTrigger();
    }

    private void CreateInteractionTrigger()
    {
        // 새 게임오브젝트 생성하여 상호작용 감지 영역으로 사용
        GameObject triggerObject = new GameObject("InteractionTrigger");
        triggerObject.transform.SetParent(this.transform);
        triggerObject.transform.localPosition = Vector3.zero;

        interactionTrigger = triggerObject.AddComponent<CircleCollider2D>();
        interactionTrigger.isTrigger = true;
        interactionTrigger.radius = interactionRadius;

        // 이 스크립트의 트리거 이벤트를 받기 위해 PlayerInteractionTrigger 컴포넌트 추가
        var triggerHandler = triggerObject.AddComponent<PlayerInteractionTrigger>();
        triggerHandler.Initialize(this);

        Debug.Log("상호작용 트리거 생성 완료");
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.Interact.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Disable();
            playerControls.UI.Interact.performed -= OnInteract;
        }
    }

    private void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
        if (inventory == null)
            Debug.LogError("씬에 InventoryManager가 없습니다!");

        if (interactionPopup == null)
        {
            Debug.LogWarning("interactionPopup이 할당되지 않았습니다! PlayerInteraction 컴포넌트에서 InteractionPopup을 할당해주세요.");
        }
        else
        {
            interactionPopup.SetActive(false);
            Debug.Log("InteractionPopup 초기화 완료");
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("OnInteract 호출됨");

        if (nearbyItem == null)
        {
            Debug.Log("nearbyItem이 null입니다");
            return;
        }

        if (inventory == null)
        {
            Debug.LogError("inventory가 null입니다");
            return;
        }

        Debug.Log($"아이템 상호작용 시도: {nearbyItem.item?.itemName}");

        try
        {
            Item itemToPickup = nearbyItem.GetItem();
            if (itemToPickup != null && inventory.AddItem(itemToPickup))
            {
                Debug.Log($"아이템 픽업 성공: {itemToPickup.itemName}");
                // nearbyItem.GetItem()에서 이미 오브젝트가 파괴되므로 여기서는 null만 설정
                nearbyItem = null;
                if (interactionPopup != null)
                    interactionPopup.SetActive(false);
            }
            else
            {
                Debug.Log("아이템 픽업 실패 - 인벤토리 공간 부족");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"OnInteract에서 오류 발생: {e.Message}");
        }
    }

    public void OnItemTriggerEnter(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D 호출됨: {other.name}");

        if (other.TryGetComponent<FieldItem>(out var fieldItem))
        {
            nearbyItem = fieldItem;
            if (interactionPopup != null)
            {
                interactionPopup.SetActive(true);
                Debug.Log($"상호작용 팝업 활성화됨: {fieldItem.item?.itemName}");
            }
            else
            {
                Debug.LogWarning("interactionPopup이 null입니다!");
            }
        }
        else
        {
            Debug.Log($"FieldItem 컴포넌트가 없습니다: {other.name}");
        }
    }

    public void OnItemTriggerExit(Collider2D other)
    {
        Debug.Log($"OnTriggerExit2D 호출됨: {other.name}");

        if (other.TryGetComponent<FieldItem>(out var fieldItem) && fieldItem == nearbyItem)
        {
            nearbyItem = null;
            if (interactionPopup != null)
            {
                interactionPopup.SetActive(false);
                Debug.Log("상호작용 팝업 비활성화됨");
            }
        }
    }
}

// 별도의 트리거 핸들러 클래스
public class PlayerInteractionTrigger : MonoBehaviour
{
    private PlayerInteraction playerInteraction;

    public void Initialize(PlayerInteraction interaction)
    {
        playerInteraction = interaction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerInteraction != null)
            playerInteraction.OnItemTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (playerInteraction != null)
            playerInteraction.OnItemTriggerExit(other);
    }
}