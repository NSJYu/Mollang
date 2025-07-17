using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 긴급 ESC 키 테스트 - 최소한의 코드로 ESC 키 작동 확인
/// Unity에서 이 스크립트를 아무 오브젝트에나 추가해서 테스트
/// </summary>
public class EmergencyESCTest : MonoBehaviour
{
    private PlayerControls controls;

    private void Awake()
    {
        Debug.Log("🚨 Emergency ESC Test 시작");

        // PlayerControls 직접 생성
        controls = new PlayerControls();

        if (controls != null)
        {
            Debug.Log("✅ PlayerControls 생성 성공");
        }
        else
        {
            Debug.LogError("❌ PlayerControls 생성 실패");
        }
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Enable();
            controls.UI.Cancel.performed += OnESCPressed;
            Debug.Log("✅ ESC 키 이벤트 등록 완료");
        }
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.UI.Cancel.performed -= OnESCPressed;
            controls.Disable();
            Debug.Log("⏸️ ESC 키 이벤트 해제");
        }
    }

    private void OnESCPressed(InputAction.CallbackContext context)
    {
        Debug.Log("🎉 ESC 키 작동 확인됨!");
        Debug.Log("Input System이 정상적으로 작동하고 있습니다.");

        // 간단한 시각적 피드백
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = Random.ColorHSV();
        }
    }

    private void Update()
    {
        // 레거시 Input도 테스트
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("🔍 레거시 Input으로도 ESC 키 감지됨");
        }
    }

    private void OnDestroy()
    {
        if (controls != null)
        {
            controls.Dispose();
        }
    }
}
