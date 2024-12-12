# Hian Unity Logger

Unity의 기본 로깅 시스템을 확장한 고급 로깅 패키지입니다.

## 개요

Unity의 기본 로깅 시스템을 확장하여 파일 로깅 기능과 로그 파일 관리 기능을 제공합니다.
스레드 안전한 로깅을 지원하며, 날짜별 자동 파일 생성과 관리 기능을 포함합니다.

## 요구사항

- Unity 2021.3 이상
- .NET Standard 2.1

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

   ![Package Manager Add Git URL](https://i.imgur.com/1tCNo66.png)
3. 다음 URL을 입력합니다:

```text
https://github.com/creator-hian/Unity-Logger.git
```

4. 'Add' 버튼을 클릭합니다.

   ![Package Manager Add Button](https://i.imgur.com/yIiD4tT.png)

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
// 파일 로그 핸들러 팩토리 생성
var factory = new FileLogHandlerFactory();

// 기본 경로에 파일 로깅 설정 (Application.persistentDataPath/Logs/log_yyyy-MM-dd.txt)
var handler = factory.CreateHandler();
LoggerManager.SetHandler(handler);

// 사용자 지정 경로에 파일 로깅 설정
var customHandler = factory.CreateHandler("CustomLogs/mylog.txt");
LoggerManager.SetHandler(customHandler);

// Unity 콘솔 출력 비활성화하고 파일에만 로깅
var fileOnlyHandler = factory.CreateHandler(enableConsoleOutput: false);
LoggerManager.SetHandler(fileOnlyHandler);

// 기본 Unity 로거로 복원
LoggerManager.ResetToDefaultHandler();
```

주요 특징:
- Factory 패턴을 통한 핸들러 생성
- 자동 경로 검증 및 디렉토리 생성
- 잘못된 경로 지정 시 기본 경로 사용
- 스레드 안전한 파일 접근
- 자동 파일 정리 및 복구

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

### 진단 로깅 (Diagnostics Logger)

시스템별로 독립적인 로그 파일을 생성하고 관리할 수 있는 진단 로깅 기능을 제공합니다.

#### 기본 사용법

```csharp
// 진단 로거 생성
string systemName = "MySystem";
IDiagnosticsLogger logger = LoggerManager.CreateDiagnosticsLogger(systemName);

// 로그 기록
logger.Log("일반 메시지");
logger.LogWarning("경고 메시지");
logger.LogError("에러 메시지");
logger.LogException(exception);

// Assert 기능
logger.Assert(condition, "조건이 false일 때 기록될 메시지");
logger.AssertOrThrow(condition, "조건이 false일 때 예외 발생");

// 수동으로 버퍼 플러시
logger.Flush();

// 사용 완료 후 정리
logger.Cleanup();
```

#### 커스텀 디렉토리 설정

```csharp
// 사용자 지정 디렉토리에 로그 파일 생성
string customDirectory = "CustomLogs/Diagnostics";
var logger = LoggerManager.CreateDiagnosticsLogger("MySystem", customDirectory);
```

#### 진단 로거 관리

```csharp
// 기존 로거 가져오기
var logger = LoggerManager.GetDiagnosticsLogger("MySystem");

// 로거 존재 여부 확인
bool exists = LoggerManager.HasDiagnosticsLogger("MySystem");

// 특정 로거 제거
LoggerManager.RemoveDiagnosticsLogger("MySystem");

// 모든 로거 정리
LoggerManager.Cleanup();
```

#### 주요 특징

- 시스템별 독립적인 로그 파일 관리
- 스레드 안전한 로깅
- 자동 버퍼 관리 (설정 가능한 임계값)
- Assert 기능 내장
- System.Diagnostics.TraceSource 기반 구현
- 커스텀 TraceListener 지원

### 디버그 로그 설정

로거의 내부 동작을 디버깅하기 위한 로그를 활성화할 수 있습니다.

#### 스크립팅 심볼을 통한 설정

Project Settings > Player > Scripting Define Symbols에 다음 심볼을 추가:

- `ENABLE_LOGGER_DEBUG`: 로거의 디버그 로그 활성화
- `DEVELOPMENT_BUILD`: Development Build 시 자동으로 디버그 로그 활성화

#### 코드를 통한 제어

```csharp
// 수동으로 디버그 로그 활성화/비활성화
LoggerManager.IsDebugLogEnabled = true;  // 활성화
LoggerManager.IsDebugLogEnabled = false; // 비활성화
```

디버그 로그가 활성화되면 로거의 내부 동작(파일 생성, 디렉토리 생성 등)이 Unity 콘솔에 출력됩니다.

## 원작성자

- [Hian](https://github.com/creator-hian)

## 라이선스

MIT License
