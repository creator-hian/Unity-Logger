using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace Hian.Logger.Handlers
{
    /// <summary>
    /// 파일 기반 로그 핸들러 클래스입니다.
    /// 로그 메시지를 파일에 기록하고 선택적으로 Unity 콘솔에도 출력합니다.
    /// 스레드 안전한 로깅을 지원하며, 파일 접근 오류를 자동으로 처리합니다.
    /// </summary>
    public class FileLogHandler : ILogHandler, IDisposable
    {
        private readonly object _lockObject = new object();
        private bool _enableConsoleOutput;
        private string _logFilePath;
        private StreamWriter _streamWriter;
        private bool _isDisposed;
        private readonly UnityEngine.ILogHandler _defaultLogHandler;

        /// <summary>
        /// FileLogHandler의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="logFilePath">로그를 기록할 파일 경로</param>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        /// <param name="defaultLogHandler">기본 로그 핸들러</param>
        /// <exception cref="ArgumentNullException">logFilePath가 null인 경우</exception>
        public FileLogHandler(string logFilePath, bool enableConsoleOutput = true, UnityEngine.ILogHandler defaultLogHandler = null)
        {
            _logFilePath = logFilePath ?? throw new ArgumentNullException(nameof(logFilePath));
            _enableConsoleOutput = enableConsoleOutput;
            _defaultLogHandler = defaultLogHandler ?? LoggerManager.GetOriginalUnityHandler();
            InitializeStream();
        }

        /// <summary>
        /// 로그 파일의 경로를 설정합니다.
        /// </summary>
        /// <param name="path">새로운 로그 파일 경로</param>
        /// <exception cref="ArgumentNullException">path가 null인 경우</exception>
        /// <exception cref="ObjectDisposedException">이미 Dispose된 경우</exception>
        public void SetLogPath(string path)
        {
            ThrowIfDisposed();
            if (path == null) throw new ArgumentNullException(nameof(path));

            lock (_lockObject)
            {
                CloseStream();
                _logFilePath = path;
                InitializeStream();
            }
        }

        /// <summary>
        /// Unity 콘솔 출력을 활성화 또는 비활성화합니다.
        /// </summary>
        /// <param name="enable">활성화 여부</param>
        /// <exception cref="ObjectDisposedException">이미 Dispose된 경우</exception>
        public void EnableConsoleOutput(bool enable)
        {
            ThrowIfDisposed();
            _enableConsoleOutput = enable;
        }

        /// <summary>
        /// 로그 메시지를 포맷팅하여 기록합니다.
        /// </summary>
        /// <param name="logType">로그 타입</param>
        /// <param name="context">로그 컨텍스트</param>
        /// <param name="format">메시지 포맷</param>
        /// <param name="args">포맷 인자</param>
        /// <exception cref="ObjectDisposedException">이미 Dispose된 경우</exception>
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            ThrowIfDisposed();

            string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string contextInfo = context != null ? $"[{context.name}] " : "";
            string message = string.Format(format, args);
            string logMessage = $"[{timeStamp}][{logType}]{contextInfo}{message}\n";

            WriteToFile(logMessage);

            if (_enableConsoleOutput && _defaultLogHandler != null)
            {
                _defaultLogHandler.LogFormat(logType, context, format, args);
            }
        }

        /// <summary>
        /// 예외를 로그에 기록합니다.
        /// </summary>
        /// <param name="exception">기록할 예외</param>
        /// <param name="context">로그 컨텍스트</param>
        /// <exception cref="ObjectDisposedException">이미 Dispose된 경우</exception>
        public void LogException(Exception exception, UnityEngine.Object context)
        {
            ThrowIfDisposed();

            string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string contextInfo = context != null ? $"[{context.name}] " : "";
            string logMessage = $"[{timeStamp}][Exception]{contextInfo}{exception}\n{exception.StackTrace}\n";

            WriteToFile(logMessage);

            if (_enableConsoleOutput && _defaultLogHandler != null)
            {
                _defaultLogHandler.LogException(exception, context);
            }
        }

        /// <summary>
        /// 파일 스트림을 초기화합니다.
        /// </summary>
        /// <exception cref="IOException">파일 스트림 생성 실패 시</exception>
        private void InitializeStream()
        {
            try
            {
                string directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _streamWriter = new StreamWriter(_logFilePath, true, Encoding.UTF8)
                {
                    AutoFlush = true
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize log file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 파일에 로그 메시지를 기록합니다.
        /// </summary>
        /// <param name="message">기록할 메시지</param>
        private void WriteToFile(string message)
        {
            lock (_lockObject)
            {
                try
                {
                    if (_streamWriter == null || _isDisposed)
                    {
                        Debug.LogError("Stream is not available");
                        return;
                    }
                    _streamWriter.Write(message);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to write to log file: {ex.Message}");
                    
                    // 스트림 재초기화 시도
                    try
                    {
                        CloseStream();
                        InitializeStream();
                        _streamWriter?.Write(message);
                    }
                    catch (Exception retryEx)
                    {
                        Debug.LogError($"Failed to retry writing to log file: {retryEx.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 스트림을 닫고 리소스를 해제합니다.
        /// </summary>
        private void CloseStream()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
        }

        /// <summary>
        /// 객체가 Dispose되었는지 확인하고, Dispose된 경우 예외를 발생시킵니다.
        /// </summary>
        /// <exception cref="ObjectDisposedException">객체가 이미 Dispose된 경우</exception>
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(FileLogHandler));
            }
        }

        /// <summary>
        /// 로그 핸들러의 리소스를 정리합니다.
        /// </summary>
        public void Cleanup()
        {
            Dispose();
        }

        /// <summary>
        /// 관리되지 않는 리소스를 해제하고 정리합니다.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            lock (_lockObject)
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    CloseStream();
                }
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 종료자입니다.
        /// </summary>
        ~FileLogHandler()
        {
            Dispose();
        }
    }
} 