using UnityEngine;

/// <summary>
/// 커서 상태 모니터링 및 디버깅 도구
/// ESC 메뉴와 인벤토리 간의 커서 충돌 문제 해결용
/// </summary>
public class CursorStateMonitor : MonoBehaviour
{
    [Header("커서 상태 모니터링")]
    [SerializeField] private bool enableLogging = true;
    [SerializeField] private bool showOnScreenInfo = false;

    private bool lastVisibleState;
    private CursorLockMode lastLockState;
    private InventoryManager inventoryManager;
    private GameMenuManager gameMenuManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        gameMenuManager = FindObjectOfType<GameMenuManager>();

        lastVisibleState = Cursor.visible;
        lastLockState = Cursor.lockState;

        if (enableLogging)
        {
            Debug.Log($"🖱️ CursorStateMonitor 시작 - visible: {Cursor.visible}, lockState: {Cursor.lockState}");
        }
    }

    private void Update()
    {
        // 커서 상태 변화 감지
        if (Cursor.visible != lastVisibleState || Cursor.lockState != lastLockState)
        {
            if (enableLogging)
            {
                Debug.Log($"🖱️ 커서 상태 변경됨: visible {lastVisibleState}→{Cursor.visible}, lock {lastLockState}→{Cursor.lockState}");
                LogUIStates();
            }

            lastVisibleState = Cursor.visible;
            lastLockState = Cursor.lockState;
        }
    }

    private void LogUIStates()
    {
        string inventoryState = "없음";
        string menuState = "없음";

        if (inventoryManager != null)
        {
            inventoryState = inventoryManager.IsOpen() ? "열림" : "닫힘";
        }

        if (gameMenuManager != null)
        {
            menuState = gameMenuManager.IsMenuOpen() ? "열림" : "닫힘";
        }

        Debug.Log($"📱 UI 상태 - 인벤토리: {inventoryState}, 메뉴: {menuState}");
    }

    [ContextMenu("🔍 현재 상태 확인")]
    public void CheckCurrentState()
    {
        Debug.Log("=== 커서 및 UI 상태 확인 ===");
        Debug.Log($"🖱️ Cursor.visible: {Cursor.visible}");
        Debug.Log($"🖱️ Cursor.lockState: {Cursor.lockState}");
        LogUIStates();
        Debug.Log("=== 상태 확인 완료 ===");
    }

    [ContextMenu("🛠️ 커서 강제 표시")]
    public void ForceCursorVisible()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("🖱️ 커서 강제 표시 완료");
    }

    [ContextMenu("🛠️ 커서 강제 숨김")]
    public void ForceCursorHidden()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("🖱️ 커서 강제 숨김 완료");
    }

    [ContextMenu("🔄 커서 상태 복구")]
    public void RestoreCursorState()
    {
        bool shouldShowCursor = false;

        // 인벤토리가 열려있으면 커서 표시
        if (inventoryManager != null && inventoryManager.IsOpen())
        {
            shouldShowCursor = true;
        }

        // 메뉴가 열려있으면 커서 표시
        if (gameMenuManager != null && gameMenuManager.IsMenuOpen())
        {
            shouldShowCursor = true;
        }

        Cursor.visible = shouldShowCursor;
        Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;

        Debug.Log($"🖱️ 커서 상태 복구 완료 - visible: {Cursor.visible}, lockState: {Cursor.lockState}");
    }

    private void OnGUI()
    {
        if (!showOnScreenInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Box("커서 상태 모니터");
        GUILayout.Label($"Visible: {Cursor.visible}");
        GUILayout.Label($"Lock State: {Cursor.lockState}");

        string inventoryState = inventoryManager != null && inventoryManager.IsOpen() ? "열림" : "닫힘";
        string menuState = gameMenuManager != null && gameMenuManager.IsMenuOpen() ? "열림" : "닫힘";

        GUILayout.Label($"인벤토리: {inventoryState}");
        GUILayout.Label($"메뉴: {menuState}");

        if (GUILayout.Button("커서 복구"))
        {
            RestoreCursorState();
        }

        GUILayout.EndArea();
    }
}
