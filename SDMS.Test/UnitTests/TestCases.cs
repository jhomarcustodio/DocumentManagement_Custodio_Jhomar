using Microsoft.AspNetCore.Http;
using SDMS.Services;
using SDMS.Test.Helpers;

namespace SDMS.Test.UnitTests;

public sealed class TestCases
{
    private readonly FileHelperService _fileHelperService;

    public TestCases()
    {
        _fileHelperService = new FileHelperService();
    }

    [Fact]
    public async Task SaveToStorageAsync_ShouldReturnGeneratedFileName_WhenCalled()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        string fileName = await _fileHelperService.SaveToStorageAsync(testFile);

        // Assert
        Assert.NotNull(fileName);
        Assert.NotEmpty(fileName);
        Assert.True(File.Exists($"Uploads/{fileName}"));
    }

    [Fact]
    public async Task GetFileBytesFromStorageAsync_ShouldReturnByteArray_WhenFileExists()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        string fileName = await _fileHelperService.SaveToStorageAsync(testFile);
        byte[]? fileBytes = await _fileHelperService.GetFileBytesFromStorageAsync(fileName);

        // Assert
        Assert.NotNull(fileBytes);
        Assert.NotEmpty(fileBytes);
    }

    [Fact]
    public async Task GetFileBytesFromStorageAsync_ShouldReturnNull_WhenFileDoesNotExists()
    {
        // Arrange
        string fileName = "nonexistentFile.pdf";

        // Act
        byte[]? fileBytes = await _fileHelperService.GetFileBytesFromStorageAsync(fileName);

        // Assert
        Assert.Null(fileBytes);
    }

    [Fact]
    public async Task DeleteFromStorage_ShouldReturnTrue_WhenFileExists()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        string fileName = await _fileHelperService.SaveToStorageAsync(testFile);
        bool? isDeleted = _fileHelperService.DeleteFromStorage(fileName);

        // Assert
        Assert.True(isDeleted);
        Assert.True(!File.Exists($"Uploads/{fileName}"));
    }

    [Fact]
    public async Task DeleteFromStorage_ShouldReturnNull_WhenFileDoesNotExists()
    {
        // Arrange
        string fileName = "nonexistentFile.pdf";

        // Act
        bool? isDeleted = _fileHelperService.DeleteFromStorage(fileName);

        // Assert
        Assert.Null(isDeleted);
    }

    [Fact]
    public async Task IsFileValid_ShouldReturnTrue_WhenFileIsValid()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        bool IsValid = FileHelperService.IsFileValid(testFile);

        // Assert
        Assert.True(IsValid);
    }

    [Fact]
    public async Task IsFileValid_ShouldReturnFalse_WhenFileNameIsNull()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            null,
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        bool IsValid = FileHelperService.IsFileValid(testFile);

        // Assert
        Assert.False(IsValid);
    }

    [Fact]
    public async Task IsFileValid_ShouldReturnFalse_WhenFileNameIsEmpty()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            string.Empty,
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        bool IsValid = FileHelperService.IsFileValid(testFile);

        // Assert
        Assert.False(IsValid);
    }

    [Fact]
    public async Task IsFileValid_ShouldReturnFalse_WhenContentTypeNotSupported()
    {
        // Arrange
        long sizeInMB = 1;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "image/jpeg",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        bool IsValid = FileHelperService.IsFileValid(testFile);

        // Assert
        Assert.False(IsValid);
    }

        [Fact]
    public async Task IsFileValid_ShouldReturnFalse_WhenFileSizeIsMoreThan10MB()
    {
        // Arrange
        long sizeInMB = 11;

        IFormFile testFile = FileHelper.CreateTestFile(
            "testFile.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        // Act
        bool IsValid = FileHelperService.IsFileValid(testFile);

        // Assert
        Assert.False(IsValid);
    }
    
}