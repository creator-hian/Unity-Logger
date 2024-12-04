using NUnit.Framework;
using UnityEngine;
using System.IO;
using System;
using Hian.Logger;

public class LogFileUtilityTests
{
    private string _testDirectory;
    private string _originalLogDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Application.temporaryCachePath, "TestLogs");
        Directory.CreateDirectory(_testDirectory);
        
        // 원래 디렉토리 저장 및 테스트 디렉토리 설정
        _originalLogDirectory = LogFileUtility.DefaultLogDirectory;
        LogFileUtility.DefaultLogDirectory = _testDirectory;
    }

    [TearDown]
    public void TearDown()
    {
        // 원래 디렉토리 복원
        LogFileUtility.DefaultLogDirectory = _originalLogDirectory;

        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Test]
    public void DeleteOldLogs_RemovesOldFiles()
    {
        // Arrange
        DateTime now = DateTime.Now;
        DateTime oldDate = now.AddDays(-10);
        
        string oldFile = Path.Combine(_testDirectory, $"log_{oldDate:yyyy-MM-dd}.txt");
        string newFile = Path.Combine(_testDirectory, $"log_{now:yyyy-MM-dd}.txt");
        
        // 파일 생성 및 생성 시간 설정
        File.WriteAllText(oldFile, "Old log");
        File.WriteAllText(newFile, "New log");
        
        // 파일 생성 시간 설정
        File.SetCreationTime(oldFile, oldDate);
        File.SetCreationTime(newFile, now);

        // Act
        int deletedCount = LogFileUtility.DeleteOldLogs(7);

        // Assert
        Assert.IsFalse(File.Exists(oldFile), "Old file should be deleted");
        Assert.IsTrue(File.Exists(newFile), "New file should still exist");
        Assert.AreEqual(1, deletedCount, "Should delete exactly one file");
    }

    [Test]
    public void DeleteAllLogs_RemovesAllFiles()
    {
        // Arrange
        DateTime now = DateTime.Now;
        string[] testFiles = new[]
        {
            Path.Combine(_testDirectory, $"log_{now.AddDays(-1):yyyy-MM-dd}.txt"),
            Path.Combine(_testDirectory, $"log_{now:yyyy-MM-dd}.txt"),
            Path.Combine(_testDirectory, $"log_{now:yyyy-MM-dd}_1.txt")
        };

        foreach (string file in testFiles)
        {
            File.WriteAllText(file, "Test log content");
        }

        // Act
        int deletedCount = LogFileUtility.DeleteAllLogs();

        // Assert
        Assert.AreEqual(testFiles.Length, deletedCount, "Should delete all log files");
        foreach (string file in testFiles)
        {
            Assert.IsFalse(File.Exists(file), $"File should be deleted: {file}");
        }
    }

    [Test]
    public void DeleteLogsByDate_RemovesSpecificDateFiles()
    {
        // Arrange
        DateTime targetDate = DateTime.Now;
        string[] targetDateFiles = new[]
        {
            Path.Combine(_testDirectory, $"log_{targetDate:yyyy-MM-dd}.txt"),
            Path.Combine(_testDirectory, $"log_{targetDate:yyyy-MM-dd}_1.txt")
        };

        string otherDateFile = Path.Combine(_testDirectory, 
            $"log_{targetDate.AddDays(-1):yyyy-MM-dd}.txt");

        // 테스트 파일 생성
        foreach (string file in targetDateFiles)
        {
            File.WriteAllText(file, "Target date log");
        }
        File.WriteAllText(otherDateFile, "Other date log");

        // Act
        int deletedCount = LogFileUtility.DeleteLogsByDate(targetDate);

        // Assert
        Assert.AreEqual(targetDateFiles.Length, deletedCount, 
            "Should delete only files from target date");
        foreach (string file in targetDateFiles)
        {
            Assert.IsFalse(File.Exists(file), 
                $"Target date file should be deleted: {file}");
        }
        Assert.IsTrue(File.Exists(otherDateFile), 
            "Other date file should still exist");
    }

    [Test]
    public void GetLogFiles_ReturnsCorrectFiles()
    {
        // Arrange
        DateTime now = DateTime.Now;
        string[] logFiles = new[]
        {
            Path.Combine(_testDirectory, $"log_{now:yyyy-MM-dd}.txt"),
            Path.Combine(_testDirectory, $"log_{now:yyyy-MM-dd}_1.txt")
        };

        string nonLogFile = Path.Combine(_testDirectory, "notALogFile.txt");

        // 테스트 파일 생성
        foreach (string file in logFiles)
        {
            File.WriteAllText(file, "Log content");
        }
        File.WriteAllText(nonLogFile, "Not a log file");

        // Act
        FileInfo[] result = LogFileUtility.GetLogFiles();

        // Assert
        Assert.AreEqual(logFiles.Length, result.Length, 
            "Should return only log files");
        foreach (var file in result)
        {
            Assert.IsTrue(file.Name.StartsWith("log_"), 
                "Should only include files starting with 'log_'");
            Assert.IsTrue(file.Name.EndsWith(".txt"), 
                "Should only include .txt files");
        }
    }
}
