# ESC 메뉴 시스템 설정 가이드

## 🎯 구현된 기능
- ESC 키를 누르면 게임 메뉴 열림/닫힘
- 메뉴가 열려있을 때 게임 일시정지
- 인벤토리가 열려있으면 인벤토리 먼저 닫기
- 설정 메뉴 지원

## 📋 Unity에서 설정해야 할 것들

### 1. GameMenuManager 스크립트 적용
1. 빈 GameObject 생성 후 이름을 "GameMenuManager"로 변경
2. `GameMenuManager.cs` 스크립트 컴포넌트 추가

### 2. 메뉴 UI 생성 (Canvas 하위에)

#### 메인 메뉴 패널
```
Canvas
├── MenuPanel (Panel)
    ├── Background (Image - 반투명 검은색)
    ├── MenuContent (Panel)
        ├── Title (Text - "게임 메뉴")
        ├── ResumeButton (Button - "게임 재개")
        ├── SettingsButton (Button - "설정")
        ├── MainMenuButton (Button - "메인 메뉴")
        └── QuitButton (Button - "게임 종료")
```

#### 설정 패널
```
Canvas
├── SettingsPanel (Panel)
    ├── Background (Image - 반투명 검은색)
    ├── SettingsContent (Panel)
        ├── Title (Text - "설정")
        ├── VolumeSlider (Slider - "음량")
        ├── GraphicsDropdown (Dropdown - "그래픽 품질")
        └── BackButton (Button - "뒤로가기")
```

### 3. GameMenuManager 컴포넌트 설정

#### Inspector에서 할당할 것들:
- **Menu Panel**: MenuPanel GameObject
- **Settings Panel**: SettingsPanel GameObject
- **Resume Button**: ResumeButton 컴포넌트
- **Settings Button**: SettingsButton 컴포넌트
- **Main Menu Button**: MainMenuButton 컴포넌트  
- **Quit Button**: QuitButton 컴포넌트

## 🎮 컨트롤

### 키 입력
- **ESC**: 메뉴 열기/닫기
- **ESC** (인벤토리 열린 상태): 인벤토리 닫기
- **ESC** (설정 메뉴 열린 상태): 설정 메뉴 닫기

### 메뉴 버튼
- **게임 재개**: 메뉴 닫고 게임 계속
- **설정**: 설정 메뉴 열기
- **메인 메뉴**: 메인 메뉴로 이동 (현재는 게임 종료)
- **게임 종료**: 애플리케이션 종료

## 🔧 주요 기능

### 게임 일시정지 시스템
- `Time.timeScale = 0f`: 게임 시간 정지
- 플레이어 입력 비활성화
- 마우스 커서 표시

### 우선순위 시스템
1. 인벤토리가 열려있으면 인벤토리 먼저 닫기
2. 설정 메뉴가 열려있으면 설정 먼저 닫기  
3. 그 다음에 메인 메뉴 토글

## 📝 확장 가능한 기능
- 키 바인딩 설정
- 음량 조절
- 그래픽 옵션
- 게임 저장/로드
- 조작법 가이드

## 🚀 테스트 방법
1. 게임 실행
2. ESC 키 누르기
3. 메뉴가 나타나고 게임이 일시정지되는지 확인
4. 각 버튼 기능 테스트
5. 인벤토리(I 키) 열고 ESC 누르면 인벤토리 먼저 닫히는지 확인
