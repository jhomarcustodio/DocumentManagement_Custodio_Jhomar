using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SDMS.DTO;
using SDMS.Test.Helpers;

namespace SDMS.Test.IntegrationTests;

public sealed class TestCases(WebAppFactory factory): IntergrationTestFixture(factory)
{
    [Fact]
    public async Task UploadDocument_ShouldReturnCreated_WhenUploadingValidFile()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile1 = FileHelper.CreateTestFile(
            "testFile1.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent1 = new StreamContent(testFile1.OpenReadStream());
        fileContent1.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request1 = new MultipartFormDataContent
        {
            { fileContent1, "file", testFile1.FileName }
        };


        // Act
        HttpResponseMessage response1 = await client.PostAsync("api/documents", request1);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.NotNull(response1.Headers.Location);
    }

    [Fact]
    public async Task UploadDocument_ShouldReturnBadRequest_WhenUploadingDocumentLargerThan10MB()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 11;

        IFormFile testFile11 = FileHelper.CreateTestFile(
            "testFile11.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent11 = new StreamContent(testFile11.OpenReadStream());
        fileContent11.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request11 = new MultipartFormDataContent
        {
            { fileContent11, "file", testFile11.FileName }
        };


        // Act
        HttpResponseMessage response11 = await client.PostAsync("api/documents", request11);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response11.StatusCode);
    }

    [Fact]
    public async Task UploadDocument_ShouldReturnBadRequest_WhenUploadingDocumentWithUnsupportedContentType()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile14 = FileHelper.CreateTestFile(
            "testFile14.pdf",
            "image/jpeg",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent14 = new StreamContent(testFile14.OpenReadStream());
        fileContent14.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var request14 = new MultipartFormDataContent
        {
            { fileContent14, "file", testFile14.FileName }
        };

        // Act
        HttpResponseMessage response14 = await client.PostAsync("api/documents", request14);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response14.StatusCode);
    }

    [Fact]
    public async Task UploadDocument_ShouldReturnConflict_WhenUploadingDocumentWithSameFilename()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile12 = FileHelper.CreateTestFile(
            "testFile12.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent12 = new StreamContent(testFile12.OpenReadStream());
        fileContent12.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request12 = new MultipartFormDataContent
        {
            { fileContent12, "file", testFile12.FileName }
        };


        // Act
        HttpResponseMessage response12 = await client.PostAsync("api/documents", request12);
        response12.EnsureSuccessStatusCode();

        HttpResponseMessage response13 = await client.PostAsync("api/documents", request12);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response13.StatusCode);
    }

    [Fact]
    public async Task GetDocuments_ShouldReturnAListOfDocuments_WhenCalled()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile2 = FileHelper.CreateTestFile(
            "testFile2.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent2 = new StreamContent(testFile2.OpenReadStream());
        fileContent2.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request2 = new MultipartFormDataContent
        {
            { fileContent2, "file", testFile2.FileName }
        };

        IFormFile testFile3 = FileHelper.CreateTestFile(
            "testFile3.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent3 = new StreamContent(testFile3.OpenReadStream());
        fileContent3.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request3 = new MultipartFormDataContent
        {
            { fileContent3, "file", testFile3.FileName }
        };

        // Act
        HttpResponseMessage response2 = await client.PostAsync("api/documents", request2);
        response2.EnsureSuccessStatusCode();
        
        HttpResponseMessage response3 = await client.PostAsync("api/documents", request3);
        response3.EnsureSuccessStatusCode();

        HttpResponseMessage response4 = await client.GetAsync("api/documents");
        var content = await response4.Content.ReadFromJsonAsync<List<DocumentDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response4.StatusCode);
        Assert.True(content!.Count > 0);
    }

    [Fact]
    public async Task GetDocumentById_ShouldReturnADocument_WhenProvidedIdIsValid()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile5 = FileHelper.CreateTestFile(
            "testFile5.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent5 = new StreamContent(testFile5.OpenReadStream());
        fileContent5.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request5 = new MultipartFormDataContent
        {
            { fileContent5, "file", testFile5.FileName }
        };


        // Act
        HttpResponseMessage response5 = await client.PostAsync("api/documents", request5);
        response5.EnsureSuccessStatusCode();
        string documentLocation = response5.Headers.Location!.ToString();

        HttpResponseMessage response6 = await client.GetAsync(documentLocation);
        var content = await response6.Content.ReadFromJsonAsync<DocumentDto>();


        // Assert
        Assert.Equal(HttpStatusCode.OK, response6.StatusCode);
        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetDocumentById_ShouldReturnNotFound_WhenProvidedIdIsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response9 = await client.GetAsync("api/documents/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response9.StatusCode);
    }

    [Fact]
    public async Task GetDocumentById_ShouldReturnBadRequest_WhenProvidedIdIsInvalid()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response15 = await client.GetAsync("api/documents/abc");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response15.StatusCode);
    }

    [Fact]
    public async Task DeleteDocumentById_ShouldReturnNoContent_WhenDeletingDocumentWithValidId()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile7 = FileHelper.CreateTestFile(
            "testFile7.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent7 = new StreamContent(testFile7.OpenReadStream());
        fileContent7.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request7 = new MultipartFormDataContent
        {
            { fileContent7, "file", testFile7.FileName }
        };


        // Act
        HttpResponseMessage response7 = await client.PostAsync("api/documents", request7);
        response7.EnsureSuccessStatusCode();
        string documentLocation = response7.Headers.Location!.ToString();

        HttpResponseMessage response8 = await client.DeleteAsync(documentLocation);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response8.StatusCode);
    }

    [Fact]
    public async Task DeleteDocumentById_ShouldReturnNotFound_WhenProvidedIdIsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response10 = await client.DeleteAsync("api/documents/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response10.StatusCode);
    }

    [Fact]
    public async Task DeleteDocumentById_ShouldReturnNotFound_WhenProvidedIdIsInvalid()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response16 = await client.DeleteAsync("api/documents/abc");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response16.StatusCode);
    }

    [Fact]
    public async Task Download_ShouldReturnFile_WhenProvidedIdIsValid()
    {
        // Arrange
        HttpClient client = CreateClient();

        long sizeInMB = 1;

        IFormFile testFile17 = FileHelper.CreateTestFile(
            "testFile17.pdf",
            "application/pdf",
            FileHelper.ConvertMBToBytes(sizeInMB));

        using var fileContent17 = new StreamContent(testFile17.OpenReadStream());
        fileContent17.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        var request17 = new MultipartFormDataContent
        {
            { fileContent17, "file", testFile17.FileName }
        };

        // Act
        HttpResponseMessage response17 = await client.PostAsync("api/documents", request17);
        response17.EnsureSuccessStatusCode();
        var content = await response17.Content.ReadFromJsonAsync<DocumentDto>();
        int contentId = content!.Id;

        HttpResponseMessage response18 = await client.GetAsync($"api/documents/{contentId}/download");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response18.StatusCode);
        Assert.Equal(testFile17.FileName, response18.Content.Headers.ContentDisposition!.FileName!.Trim('"'));
        Assert.Equal("application/pdf", response18.Content.Headers.ContentType!.MediaType);
        Assert.Equal("attachment", response18.Content.Headers.ContentDisposition!.DispositionType);
        Assert.Equal(FileHelper.ConvertMBToBytes(sizeInMB), response18.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task Download_ShouldReturnNotFound_WhenProvidedIdIsNotFound()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response19 = await client.GetAsync($"api/documents/999/download");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response19.StatusCode);
    }

    [Fact]
    public async Task Download_ShouldReturnBadRequest_WhenProvidedIdIsInvalid()
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response19 = await client.GetAsync($"api/documents/abc/download");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response19.StatusCode);
    }

}