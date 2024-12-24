using System;
using System.IO;
using Hian.Logger;
using NUnit.Framework;
using UnityEngine;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 진단 로거 테스트를 위한 기본 클래스입니다.
    /// 모든 진단 로거 테스트 클래스의 공통 기능을 제공합니다.
    /// </summary>
    public abstract class DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// 테스트 파일이 생성될 임시 디렉토리 경로
        /// </summary>
        protected string _testDirectory;

        /// <summary>
        /// 테스트에서 사용할 진단 로거 인스턴스
        /// </summary>
        protected IDiagnosticsLogger _logger;

        /// <summary>
        /// 파일 시스템 작업 후 대기 시간 (밀리초)
        /// </summary>
        protected const int WaitTimeMs = 500;

        /// <summary>
        /// 각 테스트 실행 전 환경을 초기화합니다.
        /// 임시 디렉토리를 생성하고 이전 로거를 정리합니다.
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            _testDirectory = Path.Combine(Application.temporaryCachePath, "TestDiagnostics");
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (IOException ex)
                {
                    Debug.LogError($"Failed to delete directory: {ex.Message}");
                }
            }
            _ = Directory.CreateDirectory(_testDirectory);
            LoggerManager.Cleanup();
        }

        /// <summary>
        /// 각 테스트 실행 후 리소스를 정리합니다.
        /// 로거를 정리하고 임시 디렉토리를 삭제합니다.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            _logger?.Cleanup();
            LoggerManager.Cleanup();

            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch (IOException)
                {
                    // 파일이 잠겨있을 수 있으므로 무시
                }
            }
        }

        /// <summary>
        /// 지정된 경로의 로그 파일 내용을 읽어옵니다.
        /// </summary>
        /// <param name="path">로그 파일 경로</param>
        /// <returns>로그 파일의 전체 내용</returns>
        protected string ReadLogFile(string path)
        {
            try
            {
                using FileStream fileStream = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );
                using StreamReader reader = new StreamReader(fileStream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read log file: {ex.Message}");
                throw;
            }
        }
    }
}
