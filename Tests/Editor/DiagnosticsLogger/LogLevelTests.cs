using System.IO;
using Hian.Logger;
using NUnit.Framework;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 로그 레벨별 기록 테스트를 수행합니다.
    /// </summary>
    public class LogLevelTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// 경고 레벨 로그가 올바르게 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void LogWarning_WritesWarningLevel()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string warningMessage = "Test warning message";

            // Act
            _logger.LogWarning(warningMessage);
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains("Warning", logContent);
            StringAssert.Contains(warningMessage, logContent);
        }

        /// <summary>
        /// 에러 레벨 로그가 올바르게 기록되는지 검증합니다.
        /// </summary>
        [Test]
        public void LogError_WritesErrorLevel()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            string errorMessage = "Test error message";

            // Act
            _logger.LogError(errorMessage);
            _logger.Flush();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            string logContent = ReadLogFile(logPath);
            StringAssert.Contains("Error", logContent);
            StringAssert.Contains(errorMessage, logContent);
        }
    }
}
