using Hian.Logger;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConditionalLoggerTests : ScriptingSymbolTestBase
{
    private ConditionalLogger _conditionalLogger;
    private LogType _lastLogType;
    private string _lastMessage;

    [OneTimeSetUp]
    public void TestSetUp()
    {
        base.BaseSetUp();
    }

    [OneTimeTearDown]
    public void TestTearDown()
    {
        base.BaseTearDown();
    }

    [SetUp]
    public void Setup()
    {
        _conditionalLogger = new ConditionalLogger(isEditorOrDevelopmentBuild: false);
        _lastLogType = LogType.Log;
        _lastMessage = string.Empty;
        Application.logMessageReceived += OnLogMessageReceived;
    }

    [TearDown]
    public void Teardown()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    private void OnLogMessageReceived(string message, string stackTrace, LogType type)
    {
        _lastMessage = message;
        _lastLogType = type;
    }

    [Test]
    public void LogConditionalDebug_WithDevelopmentBuild_LogsMessage()
    {
        // Arrange
        AddDevelopmentBuildSymbol();
        string testMessage = "Test Debug Message";
        _conditionalLogger = new ConditionalLogger();

        try
        {
            // Act
            _conditionalLogger.LogConditionalDebug(testMessage);

            // Assert
            Assert.That(_lastMessage, Is.EqualTo(testMessage));
            Assert.That(_lastLogType, Is.EqualTo(LogType.Log));
        }
        finally
        {
            RemoveDevelopmentBuildSymbol();
        }
    }

    [Test]
    public void LogConditionalDebug_WithoutDevelopmentBuild_DoesNotLog()
    {
        // Arrange
        _conditionalLogger = new ConditionalLogger(isEditorOrDevelopmentBuild: false);
        string testMessage = "Test Debug Message";

        // Act
        _conditionalLogger.LogConditionalDebug(testMessage);

        // Assert
        Assert.That(_lastMessage, Is.Empty);
        Assert.That(_lastLogType, Is.EqualTo(LogType.Log));
    }

    [Test]
    public void LogConditionalDebugWarning_WhenCalled_LogsCorrectWarning()
    {
        // Arrange
        _conditionalLogger = new ConditionalLogger(isEditorOrDevelopmentBuild: true);
        string testMessage = "Test Warning Message";

        // Act
        _conditionalLogger.LogConditionalDebugWarning(testMessage);

        // Assert
        Assert.That(_lastMessage, Is.EqualTo(testMessage));
        Assert.That(_lastLogType, Is.EqualTo(LogType.Warning));
    }

    [Test]
    public void LogConditionalDebugError_WhenCalled_LogsCorrectError()
    {
        // Arrange
        _conditionalLogger = new ConditionalLogger(isEditorOrDevelopmentBuild: true);
        string testMessage = "Test Error Message";
        LogAssert.Expect(LogType.Error, testMessage);

        // Act
        _conditionalLogger.LogConditionalDebugError(testMessage);

        // Assert
        Assert.That(_lastMessage, Is.EqualTo(testMessage));
        Assert.That(_lastLogType, Is.EqualTo(LogType.Error));
    }
}
