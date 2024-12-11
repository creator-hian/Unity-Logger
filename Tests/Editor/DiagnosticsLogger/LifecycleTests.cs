using Hian.Logger;
using NUnit.Framework;
using System.IO;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 로거의 생명주기 관련 테스트를 수행합니다.
    /// </summary>
    public class LifecycleTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// Cleanup 호출 시 로거가 정상적으로 제거되는지 검증합니다.
        /// </summary>
        [Test]
        public void Cleanup_RemovesLogger()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName);
            Assert.IsTrue(LoggerManager.HasDiagnosticsLogger(systemName), "Logger should exist before cleanup");

            // Act
            LoggerManager.RemoveDiagnosticsLogger(systemName);
            System.Threading.Thread.Sleep(WaitTimeMs);  // 정리 작업 대기

            // Assert
            Assert.IsFalse(LoggerManager.HasDiagnosticsLogger(systemName), "Logger should not exist after cleanup");
        }

        /// <summary>
        /// Cleanup 후 로그 시도 시 예외가 발생하지 않는지 검증합니다.
        /// </summary>
        [Test]
        public void LogAfterCleanup_HandlesGracefully()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            _logger.Cleanup();

            // Act & Assert
            Assert.DoesNotThrow(() => _logger.Log("Test message"));
        }

        /// <summary>
        /// Cleanup 시 로그 파일이 정상적으로 닫히는지 검증합니다.
        /// </summary>
        [Test]
        public void Cleanup_ClosesLogFile()
        {
            // Arrange
            string systemName = "TestSystem";
            _logger = LoggerManager.CreateDiagnosticsLogger(systemName, _testDirectory);
            _logger.Log("Test message");
            _logger.Flush();

            string logPath = Path.Combine(_testDirectory, $"{systemName}.log");
            Assert.That(File.Exists(logPath), "Log file should exist before cleanup");

            // Act
            _logger.Cleanup();
            System.Threading.Thread.Sleep(WaitTimeMs);

            // Assert
            Assert.That(File.Exists(logPath), "Log file should still exist after cleanup");
            // 파일이 닫혔으므로 삭제 가능해야 함
            Assert.DoesNotThrow(() => File.Delete(logPath));
        }
    }
} 