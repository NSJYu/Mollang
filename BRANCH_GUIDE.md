# 🌟 Mollang 프로젝트 브랜치 가이드

## 📋 브랜치 구조

### 🎯 **메인 브랜치**

-   **`main`** - 안정적인 릴리즈 버전 (프로덕션)
-   **`develop`** - 개발 통합 브랜치 (테스트 완료된 기능들)

### 🔧 **기능별 브랜치**

-   **`feature/inventory-system`** - 인벤토리 및 아이템 시스템
-   **`feature/player-movement`** - 플레이어 이동 및 애니메이션
-   **`feature/interaction-system`** - 아이템 상호작용 및 픽업 시스템
-   **`feature/ui-system`** - UI/UX 및 사용자 인터페이스

## 🚀 브랜치 사용 방법

### 1. 작업 시작하기

```bash
# 원하는 기능 브랜치로 이동
git checkout feature/inventory-system

# 최신 코드 받기
git pull origin feature/inventory-system

# 작업 수행...
```

### 2. 작업 완료 후 커밋

```bash
# 변경사항 스테이징
git add .

# 커밋 (한국어로 간략하게)
git commit -m "인벤토리 슬롯 UI 버그 수정"

# 원격 저장소에 푸시
git push origin feature/inventory-system
```

### 3. 기능 완성 후 통합

```bash
# develop 브랜치로 이동
git checkout develop

# 기능 브랜치 병합
git merge feature/inventory-system

# develop에 푸시
git push origin develop
```

## 👥 역할별 브랜치 분담

### 🎒 **Inventory System**

-   브랜치: `feature/inventory-system`
-   담당: 인벤토리 UI, 아이템 관리, 핫바 시스템, 드롭 시스템

### 🏃 **Player System**

-   브랜치: `feature/player-movement`
-   담당: 플레이어 이동, 애니메이션, 입력 시스템

### 🤝 **Interaction System**

-   브랜치: `feature/interaction-system`
-   담당: 아이템 픽업, NPC 대화, 오브젝트 상호작용

### 🎨 **UI System**

-   브랜치: `feature/ui-system`
-   담당: 메뉴 UI, HUD, 팝업, 사용자 인터페이스

## ⚠️ 주의사항

### ❌ **하지 말아야 할 것**

-   `main` 브랜치에 직접 커밋
-   다른 사람의 기능 브랜치에 무단 커밋
-   테스트 없이 `develop`에 병합

### ✅ **권장사항**

-   자주 커밋하고 푸시하기
-   큰 변경사항 전에 팀원들과 상의
-   `develop`에서 충분히 테스트 후 `main`으로 병합

## 🔥 충돌 해결

### 충돌 발생 시

```bash
# 최신 코드 받기
git pull origin develop

# 충돌 해결 후
git add .
git commit -m "충돌 해결"
git push origin your-branch
```

## 📞 도움이 필요할 때

-   일단 구글링 해보고 디코방에 질문
