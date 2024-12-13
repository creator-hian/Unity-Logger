# Changelog

All notable changes to this project will be documented in this file.

## 버전 관리 정책

이 프로젝트는 Semantic Versioning을 따릅니다:

- **Major.Minor.Patch** 형식
  - **Major**: 호환성이 깨지는 변경
  - **Minor**: 하위 호환성 있는 기능 추가
  - **Patch**: 하위 호환성 있는 버그 수정
- **최신 버전이 상단에, 이전 버전이 하단에 기록됩니다.**

## [0.2.0] - 2024-12-13

### Added

- 조건부 디버그 로깅 기능 추가
  - `IConditionalLogger` 인터페이스 및 `ConditionalLogger` 클래스 구현
  - `LoggerManager`에서 `IsDebugLogEnabled` 속성을 통해 디버그 로그 활성화/비활성화 제어
  - `DEVELOPMENT_BUILD` 스크립팅 심볼을 통한 자동 활성화
  - `LogConditionalDebug`, `LogConditionalDebugWarning`, `LogConditionalDebugError` 메서드 추가

### Changed

- `LoggerManager`에서 `_conditionalLogger` 인스턴스 관리 방식 변경
- `LoggerManager`에서 `ConditionalLog` 메서드 호출 방식 변경
  - `IConditionalLogger` 인터페이스를 통해 호출하도록 변경

### Removed

## [0.1.1] - 2024-12-12

### Added

- 디버그 로그 기능 추가
  - 스크립팅 심볼을 통한 디버그 로그 제어 (`ENABLE_LOGGER_DEBUG`)
  - Development Build 시 자동 활성화
  - 런타임에서 수동 제어 가능
  - 로거 내부 동작 추적을 위한 상세 로깅

### Changed

- DefaultDiagnosticsLogger의 디버그 메시지를 중앙 관리 방식으로 변경
- 로거 초기화 및 정리 과정의 가시성 개선

## [0.1.0] - 2024-12-11

### Added

- 진단 로깅 시스템 구현
  - System.Diagnostics 기반의 로깅 지원
  - 시스템별 독립적인 로거 관리
  - 커스텀 TraceListener 지원
  - 기본 진단 로거 제공

### Changed

- LoggerManager에서 원본 Unity 로그 핸들러 관리 방식 개선
  - 원본 핸들러를 readonly 필드로 보관
  - GetOriginalUnityHandler 메서드 추가
- FileLogHandler의 기본 핸들러 참조 방식 변경
  - 항상 원본 Unity 핸들러를 사용하도록 수정
  - 핸들러 체인 형성 방지
- 로거 관리 시스템 개선
  - Unity 로깅과 진단 로깅 분리
  - 시스템별 로거 생성/관리 API 추가
  - Factory 패턴 적용

### Fixed

- LogFileUtility의 파일 필터링 로직 개선
  - 로그 파일 패턴 검사 강화
  - 대소문자 구분 없는 확장자 비교 추가
- FileLogHandler의 null 체크 로직 개선
- 테스트 코드의 파일 정리 로직 강화

### Removed

- LoggerManager에서 SetupFileHandler 메서드 제거
  - Factory 패턴을 통한 핸들러 생성으로 대체

## [0.0.2] - 2024-12-05

### 추가

- GitHub Release를 통한 자동 패키지 퍼블리싱 기능 추가
  - 릴리스가 publish될 때 자동으로 GitHub Packages에 배포
  - GitHub Release UI를 통한 버전 관리 지원

### 변경

- GitHub Actions 워크플로우를 release 이벤트 기반으로 변경
- 패키지 배포 프로세스 자동화

## [0.0.1] - 2024-11-30

### 추가

- Unity 기본 로깅 시스템 확장 구현
- 파일 로깅 기능
  - 날짜별 자동 파일 생성
  - 동일 날짜 파일 자동 넘버링
  - 스레드 안전 로깅
  - 파일 공유 지원
- 로그 파일 관리 기능
  - 전체 로그 파일 삭제
  - 날짜별 로그 파일 삭제
  - 오래된 로그 파일 자동 정리
  - 로그 파일 목록 조회
- 오류 처리
  - 잘못된 경로 자동 기본 경로 사용
  - 파일 접근 오류 자동 복구
  - 스트림 재시도 메커니즘

### 기능

- LoggerManager: 로깅 시스템 총괄 관리
- LogFileUtility: 로그 파일 관리 유틸리티
- FileLogHandler: 파일 로깅 구현체

### 개선

- 스레드 안전성 강화
- 파일 I/O 성능 최적화
- 메모리 사용 효율화

### 문서

- README 작성
- API 문서화
- 사용 예제 추가
