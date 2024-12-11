using System;
using System.Diagnostics;

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
        
        /// <summary>
        /// 진단 로거를 초기화하고 기본 리스너를 구성합니다.
        /// </summary>
        /// <param name="sourceName">로그 소스의 이름</param>
        public virtual void Initialize(string sourceName)
        {
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
            TraceSource.Switch.Level = SourceLevels.All;
        }

        public virtual void Log(string message)
        {
            TraceSource?.TraceInformation(message);
        }

        public virtual void LogWarning(string message)
        {
            TraceSource?.TraceEvent(TraceEventType.Warning, 0, message);
        }

        public virtual void LogError(string message)
        {
            TraceSource?.TraceEvent(TraceEventType.Error, 0, message);
        }

        public virtual void LogException(Exception exception)
        {
            TraceSource?.TraceEvent(TraceEventType.Critical, 0, 
                $"Exception: {exception.Message}\nStackTrace: {exception.StackTrace}");
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
                TraceSource.Flush();
                TraceSource.Close();
                TraceSource = null;
            }
        }
    }
} 