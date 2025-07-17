using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ESC 키 문제 진단 및 해결 도구
/// ESC 키가 작동하지 않을 때 사용하는 강화된 테스트 스크립트
/// </summary>
public class ESCKeyDiagnostic : MonoBehaviour
{
    [Header("🔍 ESC 키 진단 도구")]
    [SerializeField] private bool enableDebugMode = true;
    [SerializeField] private bool useAlternativeMethod = false;

    private PlayerControls playerControls;
    private bool isTestMenuOpen = false;
    private GameObject diagnosticUI;

    private void Awake()
    {
        Debug.Log("=== ESC 키 진단 시작 ===");

        // PlayerControls 초기화 시도
        TryInitializePlayerControls();

        // 대체 UI 생성
        CreateDiagnosticUI();
    }

    private void TryInitializePlayerControls()
    {
        // 방법 1: PlayerControlsManager를 통한 초기화
        if (PlayerControlsManager.playerControls != null)
        {
            playerControls = PlayerControlsManager.playerControls;
            Debug.Log("✅ PlayerControls를 PlayerControlsManager에서 가져옴");
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerControlsManager.playerControls가 null");

            // 방법 2: 직접 생성
            try
            {
                playerControls = new PlayerControls();
                Debug.Log("✅ PlayerControls를 직접 생성함");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ PlayerControls 생성 실패: {e.Message}");
            }
        }
    }

    private void OnEnable()
    {
        if (enableDebugMode)
            Debug.Log("ESCKeyDiagnostic OnEnable 호출됨");

        if (playerControls != null)
        {
            try
            {
                playerControls.UI.Enable();
                playerControls.UI.Cancel.performed += OnESCPressed;
                Debug.Log("✅ ESC 키 이벤트 등록 성공");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ ESC 키 이벤트 등록 실패: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("❌ playerControls가 null이어서 ESC 키 등록 불가");
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            try
            {
                playerControls.UI.Cancel.performed -= OnESCPressed;
                playerControls.UI.Disable();
                Debug.Log("✅ ESC 키 이벤트 해제 성공");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ ESC 키 이벤트 해제 실패: {e.Message}");
            }
        }
    }

    private void OnESCPressed(InputAction.CallbackContext context)
    {
        Debug.Log("🎯 ESC 키 감지됨! (New Input System)");
        ToggleDiagnosticMenu();
    }

    private void Update()
    {
        // 대체 방법: 레거시 Input 시스템도 테스트
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🎯 ESC 키 감지됨! (Legacy Input System)");

            if (useAlternativeMethod)
            {
                ToggleDiagnosticMenu();
            }
        }

        // 키보드 상태 진단
        if (enableDebugMode && Input.inputString.Length > 0)
        {
            Debug.Log($"키 입력 감지: {Input.inputString}");
        }
    }

    private void ToggleDiagnosticMenu()
    {
        isTestMenuOpen = !isTestMenuOpen;

        if (diagnosticUI != null)
        {
            diagnosticUI.SetActive(isTestMenuOpen);
            Debug.Log($"🔧 진단 메뉴 {(isTestMenuOpen ? "열림" : "닫힘")}");
        }

        // 게임 일시정지
        Time.timeScale = isTestMenuOpen ? 0f : 1f;

        // 커서 제어
        Cursor.visible = isTestMenuOpen;
        Cursor.lockState = isTestMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void CreateDiagnosticUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // Canvas가 없으면 생성
            GameObject canvasObj = new GameObject("DiagnosticCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            Debug.Log("✅ 진단용 Canvas 생성됨");
        }

        // 진단 UI 패널 생성
        diagnosticUI = new GameObject("ESC_DiagnosticPanel");
        diagnosticUI.transform.SetParent(canvas.transform, false);

        UnityEngine.UI.Image panelImage = diagnosticUI.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        RectTransform panelRect = diagnosticUI.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 진단 정보 텍스트
        GameObject textObj = new GameObject("DiagnosticText");
        textObj.transform.SetParent(diagnosticUI.transform, false);

        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = "🔧 ESC 키 진단 결과\n\n" +
                   "✅ ESC 키가 정상 작동합니다!\n\n" +
                   GetDiagnosticInfo() +
                   "\n\nESC를 다시 눌러서 닫기";
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(50, 50);
        textRect.offsetMax = new Vector2(-50, -50);

        diagnosticUI.SetActive(false);
        Debug.Log("✅ 진단 UI 생성 완료");
    }

    private string GetDiagnosticInfo()
    {
        string info = "";

        // PlayerControls 상태
        info += $"PlayerControls: {(playerControls != null ? "✅" : "❌")}\n";

        // PlayerControlsManager 상태
        info += $"PlayerControlsManager: {(PlayerControlsManager.playerControls != null ? "✅" : "❌")}\n";

        // Canvas 상태
        Canvas canvas = FindObjectOfType<Canvas>();
        info += $"Canvas: {(canvas != null ? "✅" : "❌")}\n";

        // EventSystem 상태
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        info += $"EventSystem: {(eventSystem != null ? "✅" : "❌")}\n";

        return info;
    }

    [ContextMenu("🔍 시스템 진단 실행")]
    public void RunSystemDiagnostic()
    {
        Debug.Log("=== 시스템 진단 실행 ===");
        Debug.Log($"PlayerControls 상태: {(playerControls != null ? "정상" : "비정상")}");
        Debug.Log($"PlayerControlsManager 상태: {(PlayerControlsManager.playerControls != null ? "정상" : "비정상")}");
        Debug.Log($"Canvas 상태: {(FindObjectOfType<Canvas>() != null ? "정상" : "비정상")}");
        Debug.Log($"EventSystem 상태: {(FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null ? "정상" : "비정상")}");
        Debug.Log("=== 진단 완료 ===");
    }

    [ContextMenu("🧪 수동 ESC 테스트")]
    public void ManualESCTest()
    {
        Debug.Log("🧪 수동 ESC 테스트 실행");
        ToggleDiagnosticMenu();
    }

    [ContextMenu("🔄 PlayerControls 재초기화")]
    public void ReinitializePlayerControls()
    {
        Debug.Log("🔄 PlayerControls 재초기화 시도");

        // 기존 이벤트 해제
        if (playerControls != null)
        {
            playerControls.UI.Cancel.performed -= OnESCPressed;
            playerControls.Disable();
        }

        // 다시 초기화
        TryInitializePlayerControls();

        // 이벤트 다시 등록
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.Cancel.performed += OnESCPressed;
            Debug.Log("✅ PlayerControls 재초기화 완료");
        }
        else
        {
            Debug.LogError("❌ PlayerControls 재초기화 실패");
        }
    }
}
