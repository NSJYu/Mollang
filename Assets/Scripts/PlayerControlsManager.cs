using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsManager : MonoBehaviour
{
    public static PlayerControls playerControls;
    public static PlayerControlsManager instance;

    private void Awake()
    {
        // 싱글톤 패턴
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // PlayerControls 초기화
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                Debug.Log("✅ PlayerControls 초기화 완료");
            }
        }
        else
        {
            Debug.Log("PlayerControlsManager 중복 생성됨 - 파괴");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
            Debug.Log("✅ PlayerControls 활성화");
        }
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
            Debug.Log("⏸️ PlayerControls 비활성화");
        }
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Dispose();
            Debug.Log("🗑️ PlayerControls 해제");
        }
    }
}