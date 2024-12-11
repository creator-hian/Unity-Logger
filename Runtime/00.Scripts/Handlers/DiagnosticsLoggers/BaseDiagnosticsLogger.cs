using System;
using System.Diagnostics;
using UnityEngine;
using System.IO;

namespace Hian.Logger.Handlers.DiagnosticsLoggers
{
    /// <summary>
    /// System.Diagnostics 기반 로거의 기본 구현을 제공하는 추상 클래스입니다.
    /// TraceSource를 사용하여 로그를 기록합니다.
    /// </summary>
    public abstract class BaseDiagnosticsLogger : IDiagnosticsLogger
    {
        /// <summary>
        /// 로그 기록에 사용되는 TraceSource 인스턴스입니다.
        /// </summary>
        protected TraceSource TraceSource { get; private set; }
        
        protected string LogDirectory { get; private set; }

        private int _messageCount;
        private int _flushThreshold;
        private readonly object _lockObject = new object();

        /// <summary>
        /// 진단 로거를 초기화하고 기본 리스너를 구성합니다.
        /// </summary>
        /// <param name="sourceName">로그 소스의 이름</param>
        /// <param name="logDirectory">로그 파일을 저장할 디렉토리 (null인 경우 기본 디렉토리 사용)</param>
        /// <param name="flushThreshold">자동 플러시를 위한 메시지 수 임계값 (0 이하면 자동 플러시 비활성화)</param>
        public virtual void Initialize(string sourceName, string logDirectory = null, int flushThreshold = 100)
        {
            LoggerManager.DebugLog($"Initializing logger with directory: {logDirectory ?? "null"}, flushThreshold: {flushThreshold}");

            LogDirectory = logDirectory ?? Path.Combine(Application.persistentDataPath, "Diagnostics");
            _flushThreshold = flushThreshold;
            _messageCount = 0;
            
            // 디렉토리 생성 확인
            if (!Directory.Exists(LogDirectory))
            {
                try
                {
                    Directory.CreateDirectory(LogDirectory);
                    UnityEngine.Debug.Log($"Created directory: {LogDirectory}");
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Failed to create directory: {ex.Message}");
                    throw;
                }
            }

            TraceSource = new TraceSource(sourceName);
            ConfigureDefaultListeners();
        }
    

        /// <summary>
        /// 기본 리스너 설정을 구성합니다.
        /// 하위 클래스에서 재정의하여 사용자 정의 리스너를 추가할 수 있습니다.
        /// </summary>
        protected virtual void ConfigureDefaultListeners()
        {
            TraceSource.Listeners.Clear();
            TraceSource.Switch = new SourceSwitch("DefaultSwitch", "All")
            {
                Level = SourceLevels.All
            };
        }
        public virtual void Log(string message)
        {
            if (TraceSource != null)
            {
                lock (_lockObject)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0, message);
                    CheckAndFlush();
                }
            }
        }

        protected virtual void CheckAndFlush()
        {
            _messageCount++;
            if (_flushThreshold > 0 && _messageCount >= _flushThreshold)
            {
                Flush();
            }
        }

        public virtual void Flush()
        {
            lock (_lockObject)
            {
                TraceSource?.Flush();
                _messageCount = 0;
            }
        }

        public virtual void LogWarning(string message)
        {
            if (TraceSource != null)
            {
                lock (_lockObject)
                {
                    TraceSource.TraceEvent(TraceEventType.Warning, 0, message);
                    CheckAndFlush();
                }
            }
        }

        public virtual void LogError(string message)
        {
            if (TraceSource != null)
            {
                lock (_lockObject)
                {
                    TraceSource.TraceEvent(TraceEventType.Error, 0, message);
                    CheckAndFlush();
                }
            }
        }

        public virtual void LogException(Exception exception)
        {
            if (TraceSource != null)
            {
                lock (_lockObject)
                {
                    TraceSource.TraceEvent(TraceEventType.Critical, 0,
                        $"Exception: {exception.Message}\nStackTrace: {exception.StackTrace}");
                    CheckAndFlush();
                }
            }
        }
        public virtual void AddListener(TraceListener listener)
        {
            TraceSource?.Listeners.Add(listener);
        }

        public virtual void RemoveListener(TraceListener listener)
        {
            TraceSource?.Listeners.Remove(listener);
        }

        /// <summary>
        /// 로거의 리소스를 정리합니다.
        /// TraceSource를 플러시하고 닫습니다.
        /// </summary>
        public virtual void Cleanup()
        {
            if (TraceSource != null)
            {
                Flush();  // 정리 전 마지막 Flush
                TraceSource.Close();
                TraceSource = null;
            }
        }

        // IDiagnosticsLogger 인터페이스 구현
        void IDiagnosticsLogger.Initialize(string sourceName, string logDirectory)
        {
            Initialize(sourceName, logDirectory);  // 기본 flushThreshold로 호출
        }

        // GetLogFilePath를 protected virtual로 선언
        protected virtual string GetLogFilePath(string systemName)
        {
            var path = Path.Combine(LogDirectory, $"{systemName}.log");
            return path;
        }

        public virtual void Assert(bool condition, string message)
        {
            if (!condition)
            {
                lock (_lockObject)
                {
                    TraceSource?.TraceEvent(TraceEventType.Error, 0, $"Assertion failed: {message}");
                    Flush();
                }
            }
        }

        public virtual void AssertOrThrow(bool condition, string message)
        {
            if (!condition)
            {
                lock (_lockObject)
                {
                    TraceSource?.TraceEvent(TraceEventType.Critical, 0, $"Assertion failed: {message}");
                    Flush();
                    throw new Exception($"Assertion failed: {message}");
                }
            }
        }

    }
} 