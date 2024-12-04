using UnityEngine;
using System;
using System.IO;

namespace Hian.Logger
{
    /// <summary>
    /// 로그 파일 관리를 위한 유틸리티 클래스
    /// </summary>
    public static class LogFileUtility
    {
        private static string _logDirectory;

        /// <summary>
        /// 기본 로그 디렉토리 경로를 가져옵니다.
        /// </summary>
        public static string DefaultLogDirectory
        {
            get => _logDirectory ?? Path.Combine(Application.persistentDataPath, "Logs");
            set => _logDirectory = value;
        }

        /// <summary>
        /// 모든 로그 파일을 삭제합니다.
        /// </summary>
        /// <returns>삭제된 파일 수</returns>
        public static int DeleteAllLogs()
        {
            try
            {
                string[] files = Directory.GetFiles(DefaultLogDirectory, "log_*.txt");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                return files.Length;
            }
            catch (Exception e)
            {
                Debug.LogError($"로그 파일 삭제 중 오류 발생: {e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 지정된 날짜보다 오래된 로그 파일을 삭제합니다.
        /// </summary>
        /// <param name="days">보관할 일수</param>
        /// <returns>삭제된 파일 수</returns>
        public static int DeleteOldLogs(int days)
        {
            try
            {
                DateTime threshold = DateTime.Now.AddDays(-days);
                int count = 0;

                string[] files = Directory.GetFiles(DefaultLogDirectory, "log_*.txt");
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < threshold)
                    {
                        fileInfo.Delete();
                        count++;
                    }
                }
                return count;
            }
            catch (Exception e)
            {
                Debug.LogError($"오래된 로그 파일 삭제 중 오류 발생: {e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 특정 날짜의 로그 파일들을 삭제합니다.
        /// </summary>
        /// <param name="date">삭제할 로그 파일의 날짜</param>
        /// <returns>삭제된 파일 수</returns>
        public static int DeleteLogsByDate(DateTime date)
        {
            try
            {
                string datePattern = date.ToString("yyyy-MM-dd");
                string[] files = Directory.GetFiles(DefaultLogDirectory, $"log_{datePattern}*.txt");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                return files.Length;
            }
            catch (Exception e)
            {
                Debug.LogError($"날짜별 로그 파일 삭제 중 오류 발생: {e.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 현재 로그 디렉토리의 모든 로그 파일 정보를 가져옵니다.
        /// </summary>
        /// <returns>로그 파일 정보 목록</returns>
        public static FileInfo[] GetLogFiles()
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(DefaultLogDirectory);
                return dirInfo.GetFiles("log_*.txt");
            }
            catch (Exception e)
            {
                Debug.LogError($"로그 파일 목록 조회 중 오류 발생: {e.Message}");
                return new FileInfo[0];
            }
        }
    }
}