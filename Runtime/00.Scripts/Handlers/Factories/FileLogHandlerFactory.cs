using UnityEngine;
using System;
using System.IO;
using Hian.Logger.Utilities;

namespace Hian.Logger.Handlers.Factories
{
    /// <summary>
    /// 파일 기반 로그 핸들러를 생성하는 팩토리 클래스입니다.
    /// 로그 파일 경로 검증과 예외 처리를 포함합니다.
    /// </summary>
    public class FileLogHandlerFactory : ILogHandlerFactory
    {
        /// <summary>
        /// 파일 로그 핸들러를 생성합니다.
        /// </summary>
        /// <param name="path">로그 파일 경로 (null인 경우 기본 경로 사용)</param>
        /// <param name="enableConsoleOutput">Unity 콘솔 출력 활성화 여부</param>
        /// <returns>생성된 FileLogHandler 인스턴스</returns>
        /// <exception cref="ArgumentException">유효하지 않은 파일 경로인 경우</exception>
        public ILogHandler CreateHandler(string path = null, bool enableConsoleOutput = true)
        {
            try
            {
                string logFilePath = path ?? GetDefaultLogFilePath();
                ValidatePath(logFilePath);
                return new FileLogHandler(logFilePath, enableConsoleOutput, LoggerManager.GetOriginalUnityHandler());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create file handler at path: {path}\nError: {ex.Message}");
                string defaultPath = GetDefaultLogFilePath();
                return new FileLogHandler(defaultPath, enableConsoleOutput, LoggerManager.GetOriginalUnityHandler());
            }
        }

        /// <summary>
        /// 기본 로그 파일 경로를 생성합니다.
        /// </summary>
        /// <returns>생성된 기본 로그 파일 경로</returns>
        private string GetDefaultLogFilePath()
        {
            string dateString = DateTime.Now.ToString("yyyy-MM-dd");
            string baseDir = LogFileUtility.DefaultLogDirectory;
            string baseFileName = $"log_{dateString}";
            string extension = ".txt";

            string filePath = Path.Combine(baseDir, baseFileName + extension);
            int counter = 1;
            
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(baseDir, $"{baseFileName}_{counter}{extension}");
                counter++;
            }

            Directory.CreateDirectory(baseDir);
            return filePath;
        }

        /// <summary>
        /// 파일 경로의 유효성을 검사합니다.
        /// </summary>
        /// <param name="path">검사할 파일 경로</param>
        /// <exception cref="ArgumentException">경로가 너무 길거나 유효하지 않은 경우</exception>
        private void ValidatePath(string path)
        {
            if (path.Length > 260)
                throw new ArgumentException("Path is too long");

            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Invalid directory path");

            Directory.CreateDirectory(directory);
        }
    }
} 