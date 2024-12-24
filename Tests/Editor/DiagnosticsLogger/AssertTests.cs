using System;
using System.IO;
using Hian.Logger;
using NUnit.Framework;

namespace DiagnosticsLogger
{
    /// <summary>
    /// Assert 관련 테스트를 수행합니다.
    /// </summary>
    public class AssertTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// Assert 실패 시 에러 메시지가 로그에 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void Assert_WhenConditionFalse_WritesErrorMessage()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string assertMessage = "Test assertion message";

            // Act
            _logger.Assert(false, assertMessage);
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains("Assertion failed", logContent);
            StringAssert.Contains(assertMessage, logContent);
        }

        /// <summary>
        /// Assert 성공 시 로그가 기록되지 않는지 검증합니다.
        /// </summary>
        [Test]
        public void Assert_WhenConditionTrue_WritesNothing()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string assertMessage = "Test assertion message";

            // Act
            _logger.Assert(true, assertMessage);
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            if (File.Exists(logPath))
            {
                string logContent = ReadLogFile(logPath);
                StringAssert.DoesNotContain("Assertion failed", logContent);
                StringAssert.DoesNotContain(assertMessage, logContent);
            }
        }

        /// <summary>
        /// AssertOrThrow 실패 시 예외가 발생하고 로그가 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void AssertOrThrow_WhenConditionFalse_ThrowsAndWritesLog()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string assertMessage = "Test assertion message";

            // Act & Assert
            Exception ex = Assert.Throws<Exception>(
                () => _logger.AssertOrThrow(false, assertMessage)
            );
            StringAssert.Contains(assertMessage, ex.Message);

            System.Threading.Thread.Sleep(WaitTimeMs);

            // Check log file
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains("Assertion failed", logContent);
            StringAssert.Contains(assertMessage, logContent);
        }

        /// <summary>
        /// AssertOrThrow 성공 시 예외가 발생하지 않고 로그도 기록되지 않는지 검증합니다.
        /// </summary>
        [Test]
        public void AssertOrThrow_WhenConditionTrue_DoesNotThrowOrWrite()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string assertMessage = "Test assertion message";

            // Act & Assert
            Assert.DoesNotThrow(() => _logger.AssertOrThrow(true, assertMessage));
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Check log file
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            if (File.Exists(logPath))
            {
                string logContent = ReadLogFile(logPath);
                StringAssert.DoesNotContain("Assertion failed", logContent);
                StringAssert.DoesNotContain(assertMessage, logContent);
            }
        }
    }
}
