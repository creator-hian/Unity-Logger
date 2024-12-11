using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace Hian.Logger.Utilities
{
    /// <summary>
    /// 로그 파일 관리를 위한 유틸리티 클래스입니다.
    /// 로그 파일의 생성, 삭제, 정리 등의 기능을 제공합니다.
    /// </summary>
    public static class LogFileUtility
    {
        private const string LOG_DIRECTORY_NAME = "Logs";
        private const string LOG_FILE_PATTERN = "*.{0}";
        private static readonly string[] SUPPORTED_EXTENSIONS = { "log", "txt" };
        private const int MAX_PATH_LENGTH = 260;

        private static string _defaultLogDirectory = Path.Combine(Application.persistentDataPath, "Logs");

        /// <summary>
        /// 기본 로그 디렉토리 경로를 가져옵거나 설정합니다.
        /// 설정 시 자동으로 디렉토리를 생성합니다.
        /// </summary>
        /// <exception cref="ArgumentNullException">설정하려는 경로가 null인 경우</exception>
        public static string DefaultLogDirectory
        {
            get => _defaultLogDirectory;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _defaultLogDirectory = value;
                EnsureDirectoryExists(_defaultLogDirectory);
            }
        }

        /// <summary>
        /// 모든 로그 파일을 삭제합니다.
        /// </summary>
        /// <param name="throwOnError">에러 발생 시 예외를 throw할지 여부</param>
        /// <returns>삭제된 파일 수</returns>
        /// <exception cref="IOException">throwOnError가 true이고 파일 삭제 중 오류가 발생한 경우</exception>
        public static int DeleteAllLogs(bool throwOnError = false)
        {
            try
            {
                int count = 0;
                foreach (var file in GetLogFiles())
                {
                    try
                    {
                        if (IsFileAccessible(file))
                        {
                            file.Delete();
                            count++;
                        }
                    }
                    catch (Exception ex) when (!throwOnError)
                    {
                        Debug.LogWarning($"Failed to delete file {file.Name}: {ex.Message}");
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                if (throwOnError) throw;
                Debug.LogError($"Failed to delete log files: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 지정된 날짜보다 오래된 로그 파일을 삭제합니다.
        /// </summary>
        /// <param name="days">보관할 일수</param>
        /// <param name="throwOnError">에러 발생 시 예외를 throw할지 여부</param>
        /// <returns>삭제된 파일 수</returns>
        /// <exception cref="ArgumentOutOfRangeException">days가 0 이하인 경우</exception>
        /// <exception cref="IOException">throwOnError가 true이고 파일 삭제 중 오류가 발생한 경우</exception>
        public static int DeleteOldLogs(int days, bool throwOnError = false)
        {
            if (days <= 0)
                throw new ArgumentOutOfRangeException(nameof(days), "Days must be greater than 0");

            try
            {
                var cutoffDate = DateTime.Now.AddDays(-days);
                int count = 0;

                foreach (var file in GetLogFiles().Where(f => f.LastWriteTime < cutoffDate))
                {
                    try
                    {
                        if (IsFileAccessible(file))
                        {
                            file.Delete();
                            count++;
                        }
                    }
                    catch (Exception ex) when (!throwOnError)
                    {
                        Debug.LogWarning($"Failed to delete file {file.Name}: {ex.Message}");
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                if (throwOnError) throw;
                Debug.LogError($"Failed to delete old logs: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 특정 날짜의 로그 파일을 삭제합니다.
        /// </summary>
        /// <param name="date">삭제할 로그 파일의 날짜</param>
        /// <param name="throwOnError">에러 발생 시 예외를 throw할지 여부</param>
        /// <returns>삭제된 파일 수</returns>
        /// <exception cref="IOException">throwOnError가 true이고 파일 삭제 중 오류가 발생한 경우</exception>
        public static int DeleteLogsByDate(DateTime date, bool throwOnError = false)
        {
            try
            {
                string dateString = date.ToString("yyyy-MM-dd");
                int count = 0;

                foreach (var file in GetLogFiles().Where(f => f.Name.Contains(dateString)))
                {
                    try
                    {
                        if (IsFileAccessible(file))
                        {
                            file.Delete();
                            count++;
                        }
                    }
                    catch (Exception ex) when (!throwOnError)
                    {
                        Debug.LogWarning($"Failed to delete file {file.Name}: {ex.Message}");
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                if (throwOnError) throw;
                Debug.LogError($"Failed to delete logs by date: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 모든 로그 파일 정보를 가져옵니다.
        /// </summary>
        /// <param name="throwOnError">에러 발생 시 예외를 throw할지 여부</param>
        /// <returns>로그 파일 정보 배열</returns>
        /// <exception cref="IOException">throwOnError가 true이고 파일 접근 중 오류가 발생한 경우</exception>
        public static FileInfo[] GetLogFiles(bool throwOnError = false)
        {
            try
            {
                var directory = new DirectoryInfo(DefaultLogDirectory);
                if (!directory.Exists) return Array.Empty<FileInfo>();

                return directory.GetFiles()
                    .Where(f => f.Name.StartsWith("log_") && 
                               SUPPORTED_EXTENSIONS.Any(ext => f.Extension.Equals($".{ext}", 
                                   StringComparison.OrdinalIgnoreCase)))
                    .ToArray();
            }
            catch (Exception ex)
            {
                if (throwOnError) throw;
                Debug.LogError($"Failed to get log files: {ex.Message}");
                return Array.Empty<FileInfo>();
            }
        }

        /// <summary>
        /// 지정된 경로에 디렉토리가 없는 경우 생성합니다.
        /// </summary>
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create directory {path}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 파일이 접근 가능한 상태인지 확인합니다.
        /// </summary>
        private static bool IsFileAccessible(FileInfo file)
        {
            try
            {
                using (file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 