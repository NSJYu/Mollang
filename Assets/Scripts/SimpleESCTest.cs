using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ESC 메뉴 최소 테스트 버전 - 문제 해결용
/// GameMenuManager가 작동하지 않을 때 사용하는 임시 스크립트
/// </summary>
public class SimpleESCTest : MonoBehaviour
{
    [Header("간단한 ESC 테스트")]
    [SerializeField] private GameObject testMenuPanel;

    private PlayerControls playerControls;
    private bool isMenuOpen = false;

    private void Awake()
    {
        // PlayerControls 초기화
        playerControls = PlayerControlsManager.playerControls;

        // 테스트용 UI 생성
        if (testMenuPanel == null)
        {
            CreateSimpleTestUI();
        }
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.Cancel.performed += OnESCPressed;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Disable();
            playerControls.UI.Cancel.performed -= OnESCPressed;
        }
    }

    private void OnESCPressed(InputAction.CallbackContext context)
    {
        Debug.Log("🔍 SimpleESCTest: ESC 키 감지됨!");
        ToggleTestMenu();
    }

    private void ToggleTestMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (testMenuPanel != null)
        {
            testMenuPanel.SetActive(isMenuOpen);
            Debug.Log($"🔍 테스트 메뉴 {(isMenuOpen ? "열림" : "닫힘")}");
        }
        else
        {
            Debug.LogError("❌ 테스트 메뉴 패널이 없습니다!");
        }

        // 시간 정지/재개
        Time.timeScale = isMenuOpen ? 0f : 1f;

        // 커서 표시/숨김
        Cursor.visible = isMenuOpen;
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void CreateSimpleTestUI()
    {
        Debug.Log("🔍 간단한 테스트 UI 생성 중...");

        // Canvas 찾기
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas를 찾을 수 없습니다! Canvas를 먼저 생성해주세요.");
            return;
        }

        // 테스트 패널 생성
        GameObject panel = new GameObject("SimpleTestMenuPanel");
        panel.transform.SetParent(canvas.transform, false);

        // 패널 설정
        UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.7f); // 반투명 검은색

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 텍스트 생성
        GameObject textObj = new GameObject("TestText");
        textObj.transform.SetParent(panel.transform, false);

        UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = "ESC 메뉴 테스트\n\n" +
                   "✅ ESC 키가 작동합니다!\n\n" +
                   "이 메시지가 보인다면\n" +
                   "Input System이 정상 작동 중입니다.\n\n" +
                   "ESC를 다시 눌러서 닫기";
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(50, 50);
        textRect.offsetMax = new Vector2(-50, -50);

        testMenuPanel = panel;
        testMenuPanel.SetActive(false);

        Debug.Log("✅ 간단한 테스트 UI 생성 완료!");
    }

    [ContextMenu("수동 메뉴 토글 테스트")]
    public void ManualToggleTest()
    {
        Debug.Log("🔍 수동 메뉴 토글 테스트 실행");
        ToggleTestMenu();
    }

    private void Update()
    {
        // 추가 키보드 테스트 (레거시 Input)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🔍 레거시 Input으로도 ESC 키 감지됨");
        }
    }
}
