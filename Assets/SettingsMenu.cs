using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button continueButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        settingsPanel.SetActive(false);

        // 버튼에 함수 연결
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        creditsButton.onClick.AddListener(OnCredits);
        quitButton.onClick.AddListener(OnQuit);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
            Time.timeScale = isActive ? 0 : 1; // 게임 일시정지/해제
        }
    }

    public void OnContinue()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnSettings()
    {
        Debug.Log("설정 버튼 클릭됨 (추후 구현)");
        // 설정 세부 메뉴 띄우기 (추후 구현)
    }

    public void OnCredits()
    {
        Debug.Log("크레딧 버튼 클릭됨 (추후 구현)");
        // 크레딧 창 띄우기 (추후 구현)
    }

    public void OnQuit()
    {
        Application.Quit();
        Debug.Log("게임 종료");
    }
}
