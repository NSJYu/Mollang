using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Unity Editor에서만 실행되는 자동 메뉴 UI 생성기
#if UNITY_EDITOR
using UnityEditor;

public class MenuUIGenerator : MonoBehaviour
{
    [ContextMenu("자동 메뉴 UI 생성")]
    public void GenerateMenuUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다! Canvas를 먼저 생성해주세요.");
            return;
        }

        // 메인 메뉴 패널 생성
        GameObject menuPanel = CreateMenuPanel(canvas.transform);
        
        // 설정 패널 생성  
        GameObject settingsPanel = CreateSettingsPanel(canvas.transform);

        Debug.Log("ESC 메뉴 UI가 자동 생성되었습니다!");
        Debug.Log("GameMenuManager 컴포넌트에 생성된 UI 요소들을 할당해주세요.");
    }

    private GameObject CreateMenuPanel(Transform parent)
    {
        // 메인 패널
        GameObject menuPanel = new GameObject("MenuPanel");
        menuPanel.transform.SetParent(parent, false);
        
        Image panelImage = menuPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // 반투명 검은색
        
        RectTransform panelRect = menuPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 메뉴 컨텐츠 영역
        GameObject content = new GameObject("MenuContent");
        content.transform.SetParent(menuPanel.transform, false);
        
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(300, 400);

        // 제목
        CreateText("Title", "게임 메뉴", content.transform, new Vector2(0, 150), 24);

        // 버튼들
        CreateButton("ResumeButton", "게임 재개", content.transform, new Vector2(0, 50));
        CreateButton("SettingsButton", "설정", content.transform, new Vector2(0, 0));
        CreateButton("MainMenuButton", "메인 메뉴", content.transform, new Vector2(0, -50));
        CreateButton("QuitButton", "게임 종료", content.transform, new Vector2(0, -100));

        menuPanel.SetActive(false); // 초기에는 비활성화
        return menuPanel;
    }

    private GameObject CreateSettingsPanel(Transform parent)
    {
        // 설정 패널
        GameObject settingsPanel = new GameObject("SettingsPanel");
        settingsPanel.transform.SetParent(parent, false);
        
        Image panelImage = settingsPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        RectTransform panelRect = settingsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 설정 컨텐츠 영역
        GameObject content = new GameObject("SettingsContent");
        content.transform.SetParent(settingsPanel.transform, false);
        
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(300, 300);

        // 제목
        CreateText("Title", "설정", content.transform, new Vector2(0, 100), 24);

        // 설정 요소들 (나중에 확장 가능)
        CreateText("VolumeLabel", "음량", content.transform, new Vector2(0, 50), 16);
        CreateButton("BackButton", "뒤로가기", content.transform, new Vector2(0, -100));

        settingsPanel.SetActive(false); // 초기에는 비활성화
        return settingsPanel;
    }

    private GameObject CreateButton(string name, string text, Transform parent, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(200, 40);

        // 버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonObj;
    }

    private GameObject CreateText(string name, string text, Transform parent, Vector2 position, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = position;
        textRect.sizeDelta = new Vector2(250, 50);

        return textObj;
    }
}
#endif
