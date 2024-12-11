using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Threading;
using Hian.Logger;
using System.Text;
using System;
using Hian.Logger.Utilities;  // LogFileUtility를 위한 참조
using Hian.Logger.Handlers.Factories;  // FileLogHandlerFactory를 위한 참조

public class LoggerManagerTests
{
    private string _testLogPath;
    private const int WaitTimeMs = 500;
    private const int MaxRetries = 5;
    private readonly ILogHandlerFactory _fileLogHandlerFactory = new FileLogHandlerFactory();

    [SetUp]
    public void Setup()
    {
        _testLogPath = Path.Combine(Application.temporaryCachePath, "TestLogs", "test.log");
        // 이전 테스트의 파일이 남아있을 수 있으므로 정리
        if (File.Exists(_testLogPath))
        {
            File.Delete(_testLogPath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        LoggerManager.ResetToDefaultHandler();
        Thread.Sleep(WaitTimeMs); // 핸들러가 정리되기를 기다림
        
        if (File.Exists(_testLogPath))
        {
            try
            {
                File.Delete(_testLogPath);
            }
            catch (IOException)
            {
                // 파일이 아직 사용 중일 수 있으므로 무시
            }
        }
    }

    private string ReadFileWithRetry(string filePath)
    {
        Exception lastException = null;
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (IOException ex)
            {
                lastException = ex;
                Thread.Sleep(WaitTimeMs);
            }
        }
        throw new IOException($"Failed to read file after {MaxRetries} attempts", lastException);
    }

    [Test]
    public void SetHandler_WithFileHandler_CreatesLogFile()
    {
        // Arrange
        Assert.IsFalse(File.Exists(_testLogPath));

        // Act
        var handler = _fileLogHandlerFactory.CreateHandler(_testLogPath);
        LoggerManager.SetHandler(handler);
        LogAssert.Expect(LogType.Log, "Test message");
        Debug.Log("Test message");

        // Assert
        Assert.IsTrue(File.Exists(_testLogPath));
        string logContent = ReadFileWithRetry(_testLogPath);
        StringAssert.Contains("Test message", logContent);
    }

    [Test]
    public void SetHandler_WithFileHandler_WithoutPath_CreatesLogFileInDefaultLocation()
    {
        // Act
        var handler = _fileLogHandlerFactory.CreateHandler();
        LoggerManager.SetHandler(handler);
        Debug.Log("Default path test");

        // Assert
        var logFiles = LogFileUtility.GetLogFiles();
        Assert.IsTrue(logFiles.Length > 0);
    }

    [Test]
    public void ResetToDefaultHandler_RestoresOriginalHandler()
    {
        // Arrange
        var originalHandler = Debug.unityLogger.logHandler;
        var handler = _fileLogHandlerFactory.CreateHandler(_testLogPath);
        LoggerManager.SetHandler(handler);

        // Act
        LoggerManager.ResetToDefaultHandler();

        // Assert
        Assert.AreEqual(originalHandler, Debug.unityLogger.logHandler);
    }

    [Test]
    public void MultipleLogMessages_AreWrittenToFile()
    {
        // Arrange
        var handler = _fileLogHandlerFactory.CreateHandler(_testLogPath);
        LoggerManager.SetHandler(handler);

        // 예상되는 로그 메시지 설정
        LogAssert.Expect(LogType.Log, "Message 1");
        LogAssert.Expect(LogType.Warning, "Message 2");
        LogAssert.Expect(LogType.Error, "Message 3");

        // Act
        Debug.Log("Message 1");
        Debug.LogWarning("Message 2");
        Debug.LogError("Message 3");

        // 파일 쓰기가 완료될 때까지 대기
        Thread.Sleep(WaitTimeMs);

        // Assert
        string logContent = ReadFileWithRetry(_testLogPath);
        StringAssert.Contains("Message 1", logContent);
        StringAssert.Contains("Message 2", logContent);
        StringAssert.Contains("Message 3", logContent);
    }

    [Test]
    public void SetHandler_WithNullHandler_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => LoggerManager.SetHandler(null));
    }

    [Test]
    public void SetupFileHandler_WithInvalidPath_UsesDefaultPath()
    {
        // Arrange
        string invalidPath = "\\\\invalid:path";
        
        // 에러 로그 예상
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Failed to create file handler.*"));

        // Act
        var handler = _fileLogHandlerFactory.CreateHandler(invalidPath);
        LoggerManager.SetHandler(handler);
        Debug.Log("Test message");

        // Assert
        Assert.NotNull(handler);
        var logFiles = LogFileUtility.GetLogFiles();
        Assert.IsTrue(logFiles.Length > 0, "Should create log file in default location");
    }
}