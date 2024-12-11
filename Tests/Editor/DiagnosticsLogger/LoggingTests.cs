using Hian.Logger;
using NUnit.Framework;
using System;
using System.IO;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 로그 메시지 기록 관련 테스트를 수행합니다.
    /// </summary>
    public class LoggingTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// 일반 로그 메시지가 파일에 정상적으로 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void LogMessage_WritesToFile()
        {
            string systemName = "TestSystem";
            // Arrange
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string testMessage = "Test diagnostic message";

            // Act
            _logger.Log(testMessage);
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains(testMessage, logContent);
        }

        /// <summary>
        /// 예외 로그 시 스택 트레이스가 포함되는지 검증합니다.
        /// </summary>
        [Test]
        public void LogException_IncludesStackTrace()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            Exception exception = new Exception("Test exception");

            // Act
            _logger.LogException(exception);
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains("Test exception", logContent);
            StringAssert.Contains("StackTrace", logContent);
        }

        /// <summary>
        /// 여러 메시지가 동일한 파일에 순차적으로 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void MultipleMessages_AppendToSameFile()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string[] messages = { "Message 1", "Message 2", "Message 3" };

            // Act
            foreach (string message in messages)
            {
                _logger.Log(message);
            }
            
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist");
            string logContent = ReadLogFile(logPath);
            foreach (string message in messages)
            {
                StringAssert.Contains(message, logContent);
            }
        }
    }
} 