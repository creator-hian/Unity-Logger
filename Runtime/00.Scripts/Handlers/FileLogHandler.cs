using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Hian.Logger.Handlers
{
    /// <summary>
    /// 파일과 Unity 콘솔 모두에 로그를 기록하는 핸들러
    /// </summary>
    internal class FileLogHandler : ILogHandler, System.IDisposable
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();
        private readonly ILogHandler _defaultLogHandler;
        private bool _enableConsoleOutput;
        private volatile bool _isDisposed;
        
        // 로그 메시지를 위한 스레드 안전 큐
        private readonly ConcurrentQueue<string> _messageQueue;
        private readonly AutoResetEvent _messageEvent;
        private readonly Thread _writerThread;
        private StreamWriter _streamWriter;
        private FileStream _fileStream;

        /// <summary>
        /// FileLogHandler를 초기화합니다.
        /// </summary>
        /// <param name="logFilePath">로그 파일 경로</param>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        public FileLogHandler(string logFilePath, bool enableConsoleOutput = true)
        {
            _logFilePath = logFilePath;
            _defaultLogHandler = Debug.unityLogger.logHandler;
            _enableConsoleOutput = enableConsoleOutput;
            _messageQueue = new ConcurrentQueue<string>();
            _messageEvent = new AutoResetEvent(false);

            // 로그 디렉토리 생성
            string directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 스트림 초기화
            InitializeStream();

            // 백그라운드 작성기 스레드 시작
            _writerThread = new Thread(ProcessMessageQueue)
            {
                IsBackground = true,
                Name = "LogWriterThread"
            };
            _writerThread.Start();

            // Unity의 기본 로그 핸들러를 이 핸들러로 교체
            Debug.unityLogger.logHandler = this;

            // 로그 시작 메시지 기록
            EnqueueMessage($"=== Log Started: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n");
        }

        private void ProcessMessageQueue()
        {
            while (!_isDisposed)
            {
                _messageEvent.WaitOne(1000); // 1초 타임아웃으로 대기

                while (!_messageQueue.IsEmpty && !_isDisposed)
                {
                    if (_messageQueue.TryDequeue(out string message))
                    {
                        try
                        {
                            if (_streamWriter != null)
                            {
                                _streamWriter.Write(message);
                                _streamWriter.Flush();
                            }
                        }
                        catch (System.Exception e)
                        {
                            _defaultLogHandler.LogFormat(LogType.Error, null,
                                "Failed to write to log file: {0}\nError: {1}", _logFilePath, e.Message);
                            
                            // 스트림 재초기화 시도
                            lock (_lockObject)
                            {
                                CloseStream();
                                InitializeStream();
                            }
                        }
                    }
                }
            }
        }

        private void EnqueueMessage(string message)
        {
            if (!_isDisposed)
            {
                _messageQueue.Enqueue(message);
                _messageEvent.Set();
            }
        }

        private void InitializeStream()
        {
            try
            {
                _fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, 
                    FileShare.ReadWrite, 4096, FileOptions.WriteThrough);
                _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize log file: {e.Message}");
            }
        }

        private void CloseStream()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
            _fileStream?.Dispose();
            _fileStream = null;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (_isDisposed) return;

            string timeStamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string message = string.Format(format, args);
            string contextInfo = context != null ? $"[{context.name}] " : "";
            string logMessage = $"[{timeStamp}][{logType}]{contextInfo}{message}\n";

            if (_enableConsoleOutput)
            {
                _defaultLogHandler.LogFormat(logType, context, format, args);
            }

            EnqueueMessage(logMessage);
        }

        public void LogException(System.Exception exception, Object context)
        {
            string timeStamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string contextInfo = context != null ? $"[{context.name}] " : "";
            string logMessage = $"[{timeStamp}][Exception]{contextInfo}{exception}\n{exception.StackTrace}\n";

            // Unity 콘솔에 출력
            if (_enableConsoleOutput)
            {
                _defaultLogHandler.LogException(exception, context);
            }

            // 파일에 기록
            EnqueueMessage(logMessage);
        }

        /// <summary>
        /// Unity 콘솔 출력 활성화 여부를 설정합니다.
        /// </summary>
        public bool EnableConsoleOutput
        {
            get => _enableConsoleOutput;
            set => _enableConsoleOutput = value;
        }

        /// <summary>
        /// 로그 파일의 경로를 반환합니다.
        /// </summary>
        public string LogFilePath => _logFilePath;

        /// <summary>
        /// 로그 파일의 내용을 모두 지웁니다.
        /// </summary>
        public void ClearLogFile()
        {
            try
            {
                CloseStream();
                File.WriteAllText(_logFilePath, string.Empty, Encoding.UTF8);
                InitializeStream();
                EnqueueMessage($"=== Log Cleared: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n");
            }
            catch (System.Exception e)
            {
                _defaultLogHandler.LogFormat(LogType.Error, null, 
                    "Failed to clear log file: {0}\nError: {1}", _logFilePath, e.Message);
            }
        }

        /// <summary>
        /// 로그 파일의 전체 내용을 읽어옵니다.
        /// </summary>
        public string ReadLogFile()
        {
            try
            {
                // 현재 버퍼의 내용을 파일에 쓰기
                _streamWriter?.Flush();

                // 파일 읽기
                return File.Exists(_logFilePath) 
                    ? File.ReadAllText(_logFilePath, Encoding.UTF8) 
                    : string.Empty;
            }
            catch (System.Exception e)
            {
                _defaultLogHandler.LogFormat(LogType.Error, null, 
                    "Failed to read log file: {0}\nError: {1}", _logFilePath, e.Message);
                return string.Empty;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            // Unity의 기본 로그 핸들러 복원
            Debug.unityLogger.logHandler = _defaultLogHandler;
            
            _messageEvent.Set(); // 스레드 깨우기
            _writerThread.Join(2000); // 최대 2초 대기
            
            lock (_lockObject)
            {
                CloseStream();
            }
            
            _messageEvent.Dispose();
        }

        ~FileLogHandler()
        {
            Dispose();
        }
    }
} 