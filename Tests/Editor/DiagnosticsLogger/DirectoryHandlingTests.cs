using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Hian.Logger;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 로그 디렉토리 처리 관련 테스트를 수행합니다.
    /// </summary>
    public class DirectoryHandlingTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// 사용자 지정 디렉토리에 로그 파일이 생성되는지 검증합니다.
        /// </summary>
        [Test]
        public void WithCustomDirectory_CreatesLoggerInSpecifiedPath()
        {
            // Arrange
            string systemName = "TestSystem";
            string customDirectory = Path.Combine(_testDirectory, "CustomLogs");
            Directory.CreateDirectory(customDirectory);

            // Act
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, customDirectory);
            _logger.Log("Test message");
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string expectedPath = Path.Combine(customDirectory, $"{systemName}.log");
            Assert.That(File.Exists(expectedPath), "Log file should exist in custom directory");
            string content = ReadLogFile(expectedPath);
            StringAssert.Contains("Test message", content);
        }

        /// <summary>
        /// 디렉토리 미지정 시 기본 경로를 사용하는지 검증합니다.
        /// </summary>
        [Test]
        public void WithNullDirectory_UsesDefaultPath()
        {
            // Arrange
            string systemName = "TestSystem";

            // Act
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName);
            _logger.Log("Test message");
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string defaultPath = Path.Combine(Application.persistentDataPath, "Diagnostics", $"{systemName}.log");
            Assert.That(File.Exists(defaultPath), "Log file should exist in default directory");
        }

        /// <summary>
        /// 잘못된 디렉토리 경로로 생성 시 예외가 발생하는지 검증합니다.
        /// </summary>
        [Test]
        public void WithInvalidDirectory_ThrowsException()
        {
            // Arrange
            string systemName = "TestSystem";
            string invalidPath = Path.Combine("\\\\invalid:", "path");

            // 에러 로그 예상
            LogAssert.Expect(LogType.Error, new Regex("Failed to create directory.*"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                LoggerManager.CreateDiagnosticsLogger(systemName, invalidPath));
        }
    }
} 