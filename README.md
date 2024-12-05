# Hian Unity Logger

Unity의 기본 로깅 시스템을 확장한 고급 로깅 패키지입니다.

## 개요

Unity의 기본 로깅 시스템을 확장하여 파일 로깅 기능과 로그 파일 관리 기능을 제공합니다.
스레드 안전한 로깅을 지원하며, 날짜별 자동 파일 생성과 관리 기능을 포함합니다.

## 주요 기능

- Unity의 기본 로깅 시스템 확장
- 파일 로깅 지원 (자동 날짜별 파일 생성)
- 스레드 안전 로깅
- 로그 파일 관리 기능 (날짜별 삭제, 일괄 삭제 등)
- 잘못된 경로 자동 처리
- 로그 파일 자동 정리

## 설치 방법

### UPM을 통한 설치 (Git URL 사용)

#### 선행 조건

- Git 클라이언트(최소 버전 2.14.0)가 설치되어 있어야 합니다.
- Windows 사용자의 경우 `PATH` 시스템 환경 변수에 Git 실행 파일 경로가 추가되어 있어야 합니다.

#### 설치 방법 1: Package Manager UI 사용

1. Unity 에디터에서 Window > Package Manager를 엽니다.
2. 좌측 상단의 + 버튼을 클릭하고 "Add package from git URL"을 선택합니다.

   ![Package Manager Add Git URL](Document\upm-ui-giturl.png)
3. 다음 URL을 입력합니다:

```
https://github.com/creator-hian/Unity-Logger.git
```

4. 'Add' 버튼을 클릭합니다.
   
   ![Package Manager Add Button](Document\upm-ui-giturl.png)

#### 설치 방법 2: manifest.json 직접 수정

1. Unity 프로젝트의 `Packages/manifest.json` 파일을 열어 다음과 같이 dependencies 블록에 패키지를 추가하세요:

```json
{
  "dependencies": {
    "com.creator-hian.unity.logger": "https://github.com/creator-hian/Unity-Logger.git",
    ...
  }
}
```

#### 특정 버전 설치

특정 버전을 설치하려면 URL 끝에 #{version} 을 추가하세요:

```json
{
  "dependencies": {
    "com.creator-hian.unity.logger": "https://github.com/creator-hian/Unity-Logger.git#0.0.1",
    ...
  }
}
```

#### 문제 해결

설치 중 다음과 같은 오류가 발생할 경우:

- 'Git' 실행 파일을 찾을 수 없음: Git이 올바르게 설치되어 있는지 확인하세요.
- 저장소를 찾을 수 없음: URL이 올바른지 확인하세요.
- Git 관련 기타 오류: Unity 에디터를 재시작하거나 프로젝트를 다시 열어보세요.

## 참조 문서

- [Unity 공식 매뉴얼 - Git URL을 통한 패키지 설치](https://docs.unity3d.com/kr/2023.2/Manual/upm-ui-giturl.html)

## 사용 방법

### 기본 로깅

```csharp
// 기본 Unity Debug.Log 사용
Debug.Log("기본 로그 메시지");
Debug.LogWarning("경고 메시지");
Debug.LogError("에러 메시지");
```

### 파일 로깅 설정

```csharp
// 기본 경로에 파일 로깅 설정 (Application.persistentDataPath/Logs/log_yyyy-MM-dd.txt)
LoggerManager.SetupFileHandler();

// 사용자 지정 경로에 파일 로깅 설정
LoggerManager.SetupFileHandler("CustomLogs/mylog.txt");

// Unity 콘솔 출력 비활성화하고 파일에만 로깅
LoggerManager.SetupFileHandler(enableConsoleOutput: false);

// 기본 로거로 복원
LoggerManager.ResetToDefaultHandler();
```

### 로그 파일 관리

```csharp
// 모든 로그 파일 삭제
LogFileUtility.DeleteAllLogs();

// 7일 이상 된 로그 파일 삭제
LogFileUtility.DeleteOldLogs(7);

// 특정 날짜의 로그 파일 삭제
LogFileUtility.DeleteLogsByDate(DateTime.Now.AddDays(-1));

// 로그 파일 목록 조회
FileInfo[] logFiles = LogFileUtility.GetLogFiles();
```

### 종료 시 정리

```csharp
private void OnApplicationQuit()
{
    LoggerManager.Cleanup();
}
```

### 주의사항

- 파일 로깅 시 자동으로 날짜별 파일이 생성됩니다.
- 잘못된 경로 지정 시 자동으로 기본 경로를 사용합니다.
- 로그 파일은 .txt 형식으로 저장됩니다.
- 파일명 형식: log_yyyy-MM-dd.txt (날짜별 자동 생성)
- 같은 날짜에 여러 파일 생성 시 자동으로 번호가 붙습니다 (예: log_2024-01-20_1.txt)

## 원작성자

- [Hian](https://github.com/creator-hian)

## 라이선스

MIT License
