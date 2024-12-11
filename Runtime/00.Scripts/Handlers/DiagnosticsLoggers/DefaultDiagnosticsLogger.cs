using System;
using System.Diagnostics;
using UnityEngine;
using System.IO;

namespace Hian.Logger.Handlers.DiagnosticsLoggers
{
    /// <summary>
    /// System.Diagnostics 기반 로거의 기본 구현 클래스입니다.
    /// 날짜별 로그 파일을 생성하고 시간과 스레드 정보를 포함합니다.
    /// </summary>
    public class DefaultDiagnosticsLogger : BaseDiagnosticsLogger
    {
        public string SystemName
        {
            get;
            private set;
        }

        public override void Initialize(string sourceName, string logDirectory = null, int flushThreshold = 100)
        {
            SystemName = sourceName;
            base.Initialize(sourceName, logDirectory, flushThreshold);
        }

        protected override void ConfigureDefaultListeners()
        {
            base.ConfigureDefaultListeners();

            try
            {
                string logPath = GetLogFilePath(SystemName);
                LoggerManager.DebugLog($"Configuring listener for path: {logPath}");

                string directory = Path.GetDirectoryName(logPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                    LoggerManager.DebugLog($"Created directory: {directory}");
                }

                var textListener = new TextWriterTraceListener(logPath)
                {
                    TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ThreadId
                };
                
                TraceSource.Listeners.Add(textListener);
                TraceSource.Switch.Level = SourceLevels.All;
                LoggerManager.DebugLog($"Added listener for: {logPath}");
                TraceSource.Flush();
            }
            catch (Exception ex)
            {
                LoggerManager.DebugLogError($"Failed to configure listener: {ex.Message}");
                throw;
            }
        }

        public override void Cleanup()
        {
            // 리스너 정리
            if (TraceSource?.Listeners != null)
            {
                foreach (TraceListener listener in TraceSource.Listeners)
                {
                    try
                    {
                        listener.Flush();
                        listener.Close();
                    }
                    catch (Exception)
                    {
                        // 정리 중 발생한 예외는 무시
                    }
                }
            }

            base.Cleanup();
        }

        public void FlushTraceSource()
        {
            TraceSource?.Flush();
        }
    }
} 