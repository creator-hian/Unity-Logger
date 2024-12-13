using System;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using Hian.Logger.Handlers.DiagnosticsLoggers;

namespace Hian.Logger
{
    /// <summary>
    /// Unity 로깅 시스템을 관리하는 정적 클래스입니다.
    /// 로그 핸들러의 생명주기와 설정을 관리합니다.
    /// </summary>
    public static partial class LoggerManager
    {
        private static readonly UnityEngine.ILogHandler _originalUnityHandler;
        private static UnityEngine.ILogHandler _defaultHandler;
        private static ILogHandler _currentHandler;
        private static readonly Dictionary<string, IDiagnosticsLogger> _diagnosticsLoggers = new();
        private static bool _isDebugLogEnabled = false;
        private static readonly IConditionalLogger _conditionalLogger = new ConditionalLogger();

        /// <summary>
        /// 디버그 로그 활성화 여부를 설정하거나 가져옵니다.
        /// </summary>
        public static bool IsDebugLogEnabled
        {
            get => _isDebugLogEnabled;
            set => _isDebugLogEnabled = value;
        }

        static LoggerManager()
        {
            _originalUnityHandler = Debug.unityLogger.logHandler;
            _defaultHandler = _originalUnityHandler;
            _currentHandler = null;
        }

        /// <summary>
        /// 현재 활성화된 로그 핸들러를 가져옵니다.
        /// </summary>
        /// <value>현재 사용 중인 ILogHandler 인스턴스</value>
        public static ILogHandler CurrentHandler => _currentHandler;

        /// <summary>
        /// 커스텀 로그 핸들러를 설정합니다.
        /// 이전 핸들러는 자동으로 정리됩니다.
        /// </summary>
        /// <param name="handler">설정할 로그 핸들러</param>
        /// <exception cref="ArgumentNullException">handler가 null인 경우</exception>
        public static void SetHandler(ILogHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // 이전 핸들러 정리
            CleanupCurrentHandler();
            _currentHandler = handler;
            Debug.unityLogger.logHandler = handler;
        }

        /// <summary>
        /// 기본 Unity 로그 핸들러로 되돌립니다.
        /// 현재 사용 중인 커스텀 핸들러는 정리됩니다.
        /// </summary>
        public static void ResetToDefaultHandler()
        {
            CleanupCurrentHandler();
            _currentHandler = null;
            Debug.unityLogger.logHandler = _defaultHandler;
        }

        /// <summary>
        /// 현재 핸들러의 리소스를 정리합니다.
        /// </summary>
        private static void CleanupCurrentHandler()
        {
            if (_currentHandler is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        #region Diagnostics Logging

        /// <summary>
        /// 특정 시스템의 진단 로거를 가져옵니다.
        /// </summary>
        /// <param name="system">시스템 식별자</param>
        /// <returns>등록된 진단 로거 또는 null</returns>
        public static IDiagnosticsLogger GetDiagnosticsLogger(string system)
        {
            return _diagnosticsLoggers.TryGetValue(system, out var logger) ? logger : null;
        }

        /// <summary>
        /// 특정 시스템에 대한 진단 로거를 생성하고 등록합니다.
        /// </summary>
        /// <param name="system">시스템 식별자</param>
        /// <param name="logDirectory">로그 디렉토리 (null인 경우 기본 디렉토리 사용)</param>
        /// <param name="logger">사용할 커스텀 로거 (null인 경우 기본 로거 사용)</param>
        /// <returns>생성된 진단 로거</returns>
        /// <exception cref="InvalidOperationException">이미 해당 시스템의 로거가 존재하는 경우</exception>
        public static IDiagnosticsLogger CreateDiagnosticsLogger(
            string system, 
            string logDirectory = null, 
            IDiagnosticsLogger logger = null)
        {
            if (_diagnosticsLoggers.ContainsKey(system))
            {
                throw new InvalidOperationException($"Logger for system '{system}' already exists.");
            }

            var newLogger = logger ?? new DefaultDiagnosticsLogger();
            newLogger.Initialize(system, logDirectory);
            _diagnosticsLoggers[system] = newLogger;
            return newLogger;
        }

        /// <summary>
        /// 특정 시스템의 진단 로거가 존재하는지 확인합니다.
        /// </summary>
        public static bool HasDiagnosticsLogger(string system)
        {
            return _diagnosticsLoggers.ContainsKey(system);
        }

        /// <summary>
        /// 모든 진단 로거를 정리합니다.
        /// </summary>
        private static void CleanupDiagnosticsLoggers()
        {
            foreach (var logger in _diagnosticsLoggers.Values)
            {
                logger.Cleanup();
            }
            _diagnosticsLoggers.Clear();
        }

        /// <summary>
        /// 특정 시스템의 진단 로거를 제거합니다.
        /// </summary>
        /// <param name="system">제거할 시스템의 식별자</param>
        public static void RemoveDiagnosticsLogger(string system)
        {
            if (string.IsNullOrEmpty(system))
                return;

            IDiagnosticsLogger logger = null;
            lock (_diagnosticsLoggers)
            {
                if (_diagnosticsLoggers.TryGetValue(system, out logger))
                {
                    _diagnosticsLoggers.Remove(system);
                }
            }

            // Dictionary에서 제거 후 정리 작업 수행
            logger?.Cleanup();
        }

        #endregion

        /// <summary>
        /// 조건부 디버그 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void LogConditionalDebug(string message)
        {
            if (!_isDebugLogEnabled) return;
            _conditionalLogger.LogConditionalDebug(message);
        }

        /// <summary>
        /// 조건부 디버그 경고 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void LogConditionalDebugWarning(string message)
        {
            if (!_isDebugLogEnabled) return;
            _conditionalLogger.LogConditionalDebugWarning(message);
        }

        /// <summary>
        /// 조건부 디버그 에러 로그를 출력합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void LogConditionalDebugError(string message)
        {
            if (!_isDebugLogEnabled) return;
            _conditionalLogger.LogConditionalDebugError(message);
        }

        /// <summary>
        /// 애플리케이션 종료 시 로깅 시스템의 정리 작업을 수행합니다.
        /// </summary>
        public static void Cleanup()
        {
            ResetToDefaultHandler();
            CleanupDiagnosticsLoggers();
        }

        /// <summary>
        /// 원본 Unity 핸들러 가져오기 메서드 추가
        /// </summary>
        public static UnityEngine.ILogHandler GetOriginalUnityHandler()
        {
            return _originalUnityHandler;
        }
    }
}