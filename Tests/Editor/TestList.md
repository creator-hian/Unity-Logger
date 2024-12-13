# Unity-Logger Test List

## DiagnosticsLogger 관련 테스트

### AssertTests.cs

- **Assert**: 조건 실패 시 에러 메시지 기록 테스트, 성공 시 로그 미기록 테스트
- **AssertOrThrow**: 조건 실패 시 예외 발생 및 로그 기록 테스트, 성공 시 예외 미발생 및 로그 미기록 테스트

### CreationTests.cs

- **Logger Creation**: 유효한 시스템 이름으로 로거 생성 테스트, 중복된 이름으로 생성 시 예외 발생 테스트

### DirectoryHandlingTests.cs

- **Custom Directory**: 지정된 경로에 로그 파일 생성 테스트
- **Default Directory**: 디렉토리 미지정 시 기본 경로 사용 테스트
- **Invalid Directory**: 잘못된 경로로 생성 시 예외 발생 테스트

### LifecycleTests.cs

- **Cleanup**: 로거 제거 테스트, 제거 후 로그 시도 시 예외 미발생 테스트, 로그 파일 닫힘 테스트

### LoggingTests.cs

- **Normal Log**: 일반 메시지 파일 기록 테스트
- **Exception Log**: 스택 트레이스 포함 테스트
- **Multiple Logs**: 여러 메시지 순차적 기록 테스트

### LogLevelTests.cs

- **Warning Log**: 경고 레벨 로그 기록 테스트
- **Error Log**: 에러 레벨 로그 기록 테스트

## LogFileUtility 관련 테스트

### LogFileUtilityTests.cs

- **Delete Old Logs**: 지정된 일수 이전 로그 삭제 테스트
- **Delete All Logs**: 모든 로그 파일 삭제 테스트
- **Delete Logs by Date**: 특정 날짜 로그 파일 삭제 테스트
- **Get Log Files**: 로그 파일 목록 반환 테스트

## LoggerManager 관련 테스트

### LoggerManagerTests.cs

- **Set Handler**: 파일 핸들러 설정 및 로그 파일 생성 테스트
- **Set Default Handler**: 경로 없이 파일 핸들러 설정 시 기본 위치에 로그 파일 생성 테스트
- **Restore Default Handler**: 원래 핸들러 복원 테스트
- **Multiple Log Messages**: 여러 로그 메시지 파일 기록 테스트
- **Null Handler**: Null 핸들러 설정 시 예외 발생 테스트
- **Invalid Path**: 잘못된 경로로 파일 핸들러 설정 시 기본 경로 사용 테스트
