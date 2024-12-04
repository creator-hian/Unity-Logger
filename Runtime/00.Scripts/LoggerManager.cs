using UnityEngine;
using Hian.Logger.Handlers;
using System;
using System.IO;

namespace Hian.Logger
{
    /// <summary>
    /// Unity 로깅 시스템을 관리하는 정적 클래스
    /// </summary>
    public static class LoggerManager
    {
        private static ILogHandler _defaultHandler;
        private static ILogHandler _currentHandler;

        static LoggerManager()
        {
            _defaultHandler = Debug.unityLogger.logHandler;
            _currentHandler = _defaultHandler;
        }

        /// <summary>
        /// 현재 설정된 로그 핸들러를 반환합니다.
        /// </summary>
        public static ILogHandler CurrentHandler => _currentHandler;

        /// <summary>
        /// 커스텀 로그 핸들러를 설정합니다.
        /// </summary>
        public static void SetHandler(ILogHandler handler)
        {
            if (handler == null)
                throw new System.ArgumentNullException(nameof(handler));

            // 이전 핸들러 정리
            CleanupCurrentHandler();
            _currentHandler = handler;
            Debug.unityLogger.logHandler = handler;
        }

        /// <summary>
        /// 기본 로그 핸들러로 되돌립니다.
        /// </summary>
        public static void ResetToDefaultHandler()
        {
            if (_currentHandler != _defaultHandler)
            {
                CleanupCurrentHandler();
                _currentHandler = _defaultHandler;
                Debug.unityLogger.logHandler = _defaultHandler;
            }
        }

        /// <summary>
        /// 기본 로그 파일 경로를 생성합니다.
        /// 파일이 이미 존재하는 경우 번호를 추가합니다.
        /// </summary>
        private static string GetDefaultLogFilePath()
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd");
            string baseDir = LogFileUtility.DefaultLogDirectory;
            string baseFileName = $"log_{dateString}";
            string extension = ".txt";

            // 기본 파일 경로
            string filePath = Path.Combine(baseDir, baseFileName + extension);

            // 파일이 이미 존재하는 경우 번호 추가
            int counter = 1;
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(baseDir, $"{baseFileName}_{counter}{extension}");
                counter++;
            }

            // 디렉토리가 없으면 생성
            Directory.CreateDirectory(baseDir);

            return filePath;
        }

        /// <summary>
        /// 파일 로그 핸들러를 설정합니다.
        /// 잘못된 경로가 제공될 경우 기본 경로를 사용합니다.
        /// </summary>
        /// <param name="logFilePath">로그 파일 경로</param>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        /// <returns>설정된 ILogHandler 인스턴스</returns>
        public static ILogHandler SetupFileHandler(string logFilePath, bool enableConsoleOutput = true)
        {
            try
            {
                // 경로 유효성 검사
                string directory = Path.GetDirectoryName(logFilePath);
                if (string.IsNullOrEmpty(directory) || !IsValidPath(logFilePath))
                {
                    string defaultPath = GetDefaultLogFilePath();
                    Debug.LogWarning($"Invalid log file path: {logFilePath}\nUsing default path instead: {defaultPath}");
                    logFilePath = defaultPath;
                }

                var fileHandler = new FileLogHandler(logFilePath, enableConsoleOutput);
                SetHandler(fileHandler);
                return fileHandler;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to setup file handler at path: {logFilePath}\nError: {ex.Message}");
                string defaultPath = GetDefaultLogFilePath();
                Debug.LogWarning($"Using default path instead: {defaultPath}");
                return SetupFileHandler(defaultPath, enableConsoleOutput);
            }
        }

        /// <summary>
        /// 기본 경로에 파일 로그 핸들러를 설정합니다.
        /// </summary>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        /// <returns>설정된 ILogHandler 인스턴스</returns>
        public static ILogHandler SetupFileHandler(bool enableConsoleOutput = true)
        {
            string logFilePath = GetDefaultLogFilePath();
            return SetupFileHandler(logFilePath, enableConsoleOutput);
        }

        /// <summary>
        /// 경로의 유효성을 검사합니다.
        /// </summary>
        private static bool IsValidPath(string path)
        {
            try
            {
                // 경로 길이 검사
                if (path.Length > 260) return false;

                // 잘못된 문자 검사
                Path.GetFullPath(path);

                // 디렉토리 접근 권한 검사
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void CleanupCurrentHandler()
        {
            if (_currentHandler is System.IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 애플리케이션 종료 시 정리 작업을 수행합니다.
        /// </summary>
        public static void Cleanup()
        {
            ResetToDefaultHandler();
        }
    }
}