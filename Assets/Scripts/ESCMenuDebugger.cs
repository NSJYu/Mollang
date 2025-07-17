using UnityEngine;

public class ESCMenuDebugger : MonoBehaviour
{
    [Header("ESC 메뉴 시스템 진단")]
    [SerializeField] private bool showDetailedInfo = true;

    private void Start()
    {
        if (showDetailedInfo)
        {
            PerformSystemCheck();
        }
    }

    [ContextMenu("ESC 메뉴 시스템 진단")]
    public void PerformSystemCheck()
    {
        Debug.Log("=== ESC 메뉴 시스템 진단 시작 ===");

        // 1. GameMenuManager 확인
        GameMenuManager menuManager = FindObjectOfType<GameMenuManager>();
        if (menuManager == null)
        {
            Debug.LogError("❌ GameMenuManager를 찾을 수 없습니다!");
            Debug.LogError("해결방법: Hierarchy에서 빈 GameObject 생성 → GameMenuManager.cs 컴포넌트 추가");
            return;
        }
        else
        {
            Debug.Log("✅ GameMenuManager 발견됨");
        }

        // 2. Canvas 확인
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas를 찾을 수 없습니다!");
            Debug.LogError("해결방법: Hierarchy 우클릭 → UI → Canvas");
            return;
        }
        else
        {
            Debug.Log("✅ Canvas 발견됨");
        }

        // 3. PlayerControls 확인
        if (PlayerControlsManager.playerControls == null)
        {
            Debug.LogError("❌ PlayerControls가 초기화되지 않았습니다!");
            Debug.LogError("해결방법: PlayerControlsManager가 씬에 있는지 확인");
        }
        else
        {
            Debug.Log("✅ PlayerControls 초기화됨");
        }

        // 4. 메뉴 패널 확인
        GameObject menuPanel = GameObject.Find("MenuPanel");
        if (menuPanel == null)
        {
            Debug.LogWarning("⚠️ MenuPanel을 찾을 수 없습니다!");
            Debug.LogWarning("해결방법: MenuUIGenerator를 사용하거나 수동으로 UI 생성");
        }
        else
        {
            Debug.Log("✅ MenuPanel 발견됨");
        }

        // 5. 설정 패널 확인
        GameObject settingsPanel = GameObject.Find("SettingsPanel");
        if (settingsPanel == null)
        {
            Debug.LogWarning("⚠️ SettingsPanel을 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log("✅ SettingsPanel 발견됨");
        }

        Debug.Log("=== ESC 메뉴 시스템 진단 완료 ===");
        Debug.Log("문제가 있다면 위의 해결방법을 따라해보세요.");
    }

    [ContextMenu("ESC 키 테스트")]
    public void TestESCKey()
    {
        Debug.Log("수동 ESC 키 테스트 실행 중...");

        GameMenuManager menuManager = FindObjectOfType<GameMenuManager>();
        if (menuManager != null)
        {
            // GameMenuManager의 ToggleMenu 메서드를 직접 호출해서 테스트
            // (private이므로 리플렉션 사용)
            var method = typeof(GameMenuManager).GetMethod("ToggleMenu",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(menuManager, null);
                Debug.Log("✅ 메뉴 토글 테스트 완료");
            }
            else
            {
                Debug.LogError("❌ ToggleMenu 메서드를 찾을 수 없습니다");
            }
        }
        else
        {
            Debug.LogError("❌ GameMenuManager를 찾을 수 없어서 테스트 불가");
        }
    }

    private void Update()
    {
        // 간단한 ESC 키 감지 (테스트용)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🔍 ESC 키가 감지되었습니다 (레거시 Input 시스템)");
        }
    }
}
