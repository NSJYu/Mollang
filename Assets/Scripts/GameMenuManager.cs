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
    
    private PlayerControls playerControls;
    private bool isMenuOpen = false;
    private InventoryManager inventoryManager;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerControls = PlayerControlsManager.playerControls;
        
        // 다른 시스템들 참조
        inventoryManager = FindObjectOfType<InventoryManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Enable();
            playerControls.UI.Cancel.performed += OnEscapePressed;
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.UI.Disable();
            playerControls.UI.Cancel.performed -= OnEscapePressed;
        }
    }

    private void Start()
    {
        // 메뉴 패널 초기 비활성화
        if (menuPanel != null)
            menuPanel.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // 버튼 이벤트 연결
        SetupButtons();
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
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
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
