using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("메뉴 UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("메뉴 버튼들")]
    [SerializeField] private UnityEngine.UI.Button resumeButton;
    [SerializeField] private UnityEngine.UI.Button settingsButton;
    [SerializeField] private UnityEngine.UI.Button mainMenuButton;
    [SerializeField] private UnityEngine.UI.Button quitButton;

    [Header("설정 메뉴 버튼들")]
    [SerializeField] private UnityEngine.UI.Button backButton;

    private PlayerControls playerControls;
    private bool isMenuOpen = false;
    private InventoryManager inventoryManager;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        // PlayerControlsManager가 있는지 확인하고 없으면 생성
        EnsurePlayerControlsManager();

        // 다른 시스템들 참조
        inventoryManager = FindObjectOfType<InventoryManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void EnsurePlayerControlsManager()
    {
        // PlayerControlsManager 찾기
        PlayerControlsManager manager = FindObjectOfType<PlayerControlsManager>();
        if (manager == null)
        {
            Debug.LogWarning("⚠️ PlayerControlsManager가 없습니다. 자동 생성합니다.");
            GameObject managerObj = new GameObject("PlayerControlsManager");
            managerObj.AddComponent<PlayerControlsManager>();
        }

        // PlayerControls 참조 시도
        playerControls = PlayerControlsManager.playerControls;
        if (playerControls == null)
        {
            Debug.LogError("❌ PlayerControls가 초기화되지 않았습니다!");
        }
    }

    private void OnEnable()
    {
        // Start에서 이벤트 등록하도록 변경
    }

    private void OnDisable()
    {
        UnregisterESCInput();
    }

    private void RegisterESCInput()
    {
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.Cancel.performed += OnEscapePressed;
            Debug.Log("✅ ESC 키 이벤트 등록 완료");
        }
        else
        {
            Debug.LogError("❌ PlayerControls가 null이라서 ESC 키 이벤트 등록 실패");
        }
    }

    private void UnregisterESCInput()
    {
        if (playerControls != null)
        {
            playerControls.UI.Cancel.performed -= OnEscapePressed;
            Debug.Log("⏸️ ESC 키 이벤트 해제 완료");
        }
    }

    private void Start()
    {
        Debug.Log("=== GameMenuManager Start 시작 ===");

        // PlayerControls 재확인 및 ESC 키 이벤트 등록
        if (playerControls == null)
        {
            playerControls = PlayerControlsManager.playerControls;
        }
        RegisterESCInput();

        // 컴포넌트 참조 확인
        Debug.Log($"playerControls: {playerControls != null}");
        Debug.Log($"inventoryManager: {inventoryManager != null}");
        Debug.Log($"playerMovement: {playerMovement != null}");

        // UI 요소 확인
        Debug.Log($"menuPanel: {menuPanel != null}");
        Debug.Log($"settingsPanel: {settingsPanel != null}");
        Debug.Log($"resumeButton: {resumeButton != null}");
        Debug.Log($"settingsButton: {settingsButton != null}");
        Debug.Log($"mainMenuButton: {mainMenuButton != null}");
        Debug.Log($"quitButton: {quitButton != null}");
        Debug.Log($"backButton: {backButton != null}");

        // 메뉴 패널 초기 비활성화
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            Debug.Log("MenuPanel 비활성화 완료");
        }
        else
        {
            Debug.LogError("❌ MenuPanel이 할당되지 않았습니다! Inspector에서 MenuPanel을 할당해주세요.");
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            Debug.Log("SettingsPanel 비활성화 완료");
        }
        else
        {
            Debug.LogWarning("⚠️ SettingsPanel이 할당되지 않았습니다.");
        }

        // 버튼 이벤트 연결
        SetupButtons();

        Debug.Log("=== GameMenuManager Start 완료 ===");
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // 설정 메뉴 뒤로가기 버튼
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseSettings);
            Debug.Log("✅ 뒤로가기 버튼 이벤트 연결 완료");
        }
        else
        {
            Debug.LogWarning("⚠️ 뒤로가기 버튼이 할당되지 않았습니다!");
        }
    }

    private void OnEscapePressed(InputAction.CallbackContext context)
    {
        Debug.Log("ESC 키 눌림");

        // 인벤토리가 열려있으면 인벤토리 먼저 닫기
        if (inventoryManager != null && inventoryManager.IsOpen())
        {
            inventoryManager.CloseInventory();
            return;
        }

        // 설정 메뉴가 열려있으면 설정 먼저 닫기
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        // 메뉴 토글
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
        {
            menuPanel.SetActive(isMenuOpen);
        }

        // 게임 일시정지/재개
        SetGamePaused(isMenuOpen);

        Debug.Log($"게임 메뉴 {(isMenuOpen ? "열림" : "닫힘")}");
    }

    private void SetGamePaused(bool paused)
    {
        // 시간 스케일 조정
        Time.timeScale = paused ? 0f : 1f;

        // 플레이어 입력 비활성화/활성화
        if (playerMovement != null)
        {
            if (paused)
                playerMovement.DisableGameplayInput();
            else
                playerMovement.EnableGameplayInput();
        }

        // 마우스 커서 표시/숨김
        if (paused)
        {
            // 메뉴가 열릴 때는 항상 커서 표시
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // 게임 재개 시: 다른 UI가 열려있는지 확인
            bool shouldShowCursor = false;

            // 인벤토리가 열려있으면 커서 유지
            if (inventoryManager != null && inventoryManager.IsOpen())
            {
                shouldShowCursor = true;
                Debug.Log("🖱️ 인벤토리가 열려있어서 커서 유지");
            }

            // 설정 패널이 열려있으면 커서 유지
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                shouldShowCursor = true;
                Debug.Log("🖱️ 설정 패널이 열려있어서 커서 유지");
            }

            Cursor.visible = shouldShowCursor;
            Cursor.lockState = shouldShowCursor ? CursorLockMode.None : CursorLockMode.Locked;

            Debug.Log($"🖱️ 커서 상태: visible={Cursor.visible}, lockState={Cursor.lockState}");
        }
    }

    public void ResumeGame()
    {
        Debug.Log("게임 재개");
        isMenuOpen = false;

        if (menuPanel != null)
            menuPanel.SetActive(false);

        SetGamePaused(false);
    }

    public void OpenSettings()
    {
        Debug.Log("설정 메뉴 열기");

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Debug.Log("설정 메뉴 닫기");

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Debug.Log("메인 메뉴로 이동");

        // 시간 스케일 복원
        Time.timeScale = 1f;

        // 메인 메뉴 씬으로 이동 (씬이 있다면)
        // SceneManager.LoadScene("MainMenu");

        // 현재는 게임 종료로 대체
        QuitGame();
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // 다른 스크립트에서 메뉴 상태 확인용
    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }
}
