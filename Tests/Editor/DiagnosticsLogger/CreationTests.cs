using Hian.Logger;
using NUnit.Framework;
using System;

namespace DiagnosticsLogger
{
    /// <summary>
    /// 진단 로거 생성 관련 테스트를 수행합니다.
    /// </summary>
    public class CreationTests : DiagnosticsLoggerTestBase
    {
        /// <summary>
        /// 유효한 시스템 이름으로 로거를 생성할 수 있는지 검증합니다.
        /// </summary>
        [Test]
        public void WithValidSystem_CreatesLogger()
        {
            // Act
            IDiagnosticsLogger logger = LoggerManager.CreateDiagnosticsLogger("TestSystem");

            // Assert
            Assert.NotNull(logger);
            Assert.IsTrue(LoggerManager.HasDiagnosticsLogger("TestSystem"));
        }

        /// <summary>
        /// 중복된 시스템 이름으로 로거 생성 시 예외가 발생하는지 검증합니다.
        /// </summary>
        [Test]
        public void WithDuplicateSystem_ThrowsException()
        {
            // Arrange
            LoggerManager.CreateDiagnosticsLogger("TestSystem");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                LoggerManager.CreateDiagnosticsLogger("TestSystem"));
        }
    }
} 