using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Hian.Logger;

public class LoggerManagerConditionalTests : ScriptingSymbolTestBase
    {
        private UnityEngine.LogType _lastLogType;
        private string _lastMessage;

        [SetUp]
        public void Setup()
        {
            _lastLogType = UnityEngine.LogType.Log;
            _lastMessage = string.Empty;
            LoggerManager.IsDebugLogEnabled = false;
            Application.logMessageReceived += OnLogMessageReceived;
        }

        [TearDown]
        public void Teardown()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            LoggerManager.IsDebugLogEnabled = false;
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            _lastMessage = message;
            _lastLogType = type;
        }

        [Test]
        public void LogConditionalDebug_InDevelopmentBuild_LogsMessage()
        {
            // Arrange
            AddDevelopmentBuildSymbol();
            string testMessage = "Test Debug Message";
            LoggerManager.IsDebugLogEnabled = true;

            try
            {
                // Act
                LoggerManager.LogConditionalDebug(testMessage);

                // Assert
                Assert.That(_lastMessage, Is.EqualTo(testMessage));
                Assert.That(_lastLogType, Is.EqualTo(UnityEngine.LogType.Log));
            }
            finally
            {
                RemoveDevelopmentBuildSymbol();
            }
        }

        [Test]
        public void LogConditionalDebugWarning_WhenDebugEnabled_LogsWarning()
        {
            // Arrange
            AddDevelopmentBuildSymbol();
            string testMessage = "Test Warning Message";
            LoggerManager.IsDebugLogEnabled = true;

            try
            {
                // Act
                LoggerManager.LogConditionalDebugWarning(testMessage);

                // Assert
                Assert.That(_lastMessage, Is.EqualTo(testMessage));
                Assert.That(_lastLogType, Is.EqualTo(UnityEngine.LogType.Warning));
            }
            finally
            {
                RemoveDevelopmentBuildSymbol();
            }
        }

        [Test]
        public void LogConditionalDebugError_WhenDebugEnabled_LogsError()
        {
            // Arrange
            AddDevelopmentBuildSymbol();
            string testMessage = "Test Error Message";
            LoggerManager.IsDebugLogEnabled = true;
            LogAssert.Expect(LogType.Error, testMessage);

            try
            {
                // Act
                LoggerManager.LogConditionalDebugError(testMessage);

                // Assert
                Assert.That(_lastMessage, Is.EqualTo(testMessage));
                Assert.That(_lastLogType, Is.EqualTo(UnityEngine.LogType.Error));
            }
            finally
            {
                RemoveDevelopmentBuildSymbol();
            }
        }

        [Test]
        public void LogConditionalDebug_WhenDebugDisabled_DoesNotLog()
        {
            // Arrange
            RemoveEditorAndDevelopmentSymbols();
            string testMessage = "Test Debug Message";
            LoggerManager.IsDebugLogEnabled = false;

            try
            {
                // Act
                LoggerManager.LogConditionalDebug(testMessage);

                // Assert
                Assert.That(_lastMessage, Is.Empty);
                Assert.That(_lastLogType, Is.EqualTo(UnityEngine.LogType.Log));
            }
            finally
            {
                RestoreEditorSymbol();
            }
        }

        [Test]
        public void IsDebugLogEnabled_ToggleValue_ReflectsCorrectly()
        {
            // Act & Assert
            LoggerManager.IsDebugLogEnabled = true;
            Assert.That(LoggerManager.IsDebugLogEnabled, Is.True);

            LoggerManager.IsDebugLogEnabled = false;
            Assert.That(LoggerManager.IsDebugLogEnabled, Is.False);
        }
    }
