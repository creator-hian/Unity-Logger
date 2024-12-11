using NUnit.Framework;
using UnityEngine;
using System.IO;
using System;
using Hian.Logger;
using Hian.Logger.Utilities;

public class LogFileUtilityTests
{
    private string _testDirectory;
    private string _originalLogDirectory;

    [SetUp]
    public void Setup()
    {
        _testDirectory = Path.Combine(Application.temporaryCachePath, "TestLogs");
        
        // 테스트 디렉토리 초기화
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
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

        // 테스트 디렉토리 정리
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch (IOException)
            {
                // 파일이 잠겨있을 수 있으므로 무시
            }
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
        
        // 파일 생성
        File.WriteAllText(oldFile, "Old log");
        File.WriteAllText(newFile, "New log");
        
        // 파일 시간 설정 (LastWriteTime 사용)
        File.SetLastWriteTime(oldFile, oldDate);
        File.SetLastWriteTime(newFile, now);

        // 파일 존재 확인
        Assert.IsTrue(File.Exists(oldFile), "Old file should exist before deletion");
        Assert.IsTrue(File.Exists(newFile), "New file should exist before deletion");

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

        // 테스트 파일 생성 전 디렉토리 정리
        foreach (var file in Directory.GetFiles(_testDirectory))
        {
            File.Delete(file);
        }

        // 테스트 파일 생성
        foreach (string file in logFiles)
        {
            File.WriteAllText(file, "Log content");
        }
        File.WriteAllText(nonLogFile, "Not a log file");

        // Act
        FileInfo[] result = LogFileUtility.GetLogFiles();

        // Assert
        Assert.AreEqual(logFiles.Length, result.Length, "Should return only log files");
        foreach (var file in result)
        {
            Assert.IsTrue(file.Name.StartsWith("log_"), "Should only include files starting with 'log_'");
            Assert.IsTrue(file.Name.EndsWith(".txt"), "Should only include .txt files");
        }
    }
}
