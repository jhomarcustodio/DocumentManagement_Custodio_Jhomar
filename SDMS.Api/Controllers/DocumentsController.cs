using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDMS.Database;
using SDMS.DTO;
using SDMS.Entities;
using SDMS.Services;

namespace  SDMS.Controllers;

[ApiController]
[Route("api/documents")]
public sealed class DocumentsController(
    DocumentDbContext dbContext
    ): ControllerBase {

    [HttpGet]
    public async Task<ActionResult<List<DocumentDto>>> GetDocuments()
    {
        List<DocumentDto> documents = await dbContext.Documents
            .OrderByDescending(d => d.UploadedAtUtc)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                OriginalFilename = d.OriginalFilename,
                StoredFilename = d.StoredFilename,
                ContentType = d.ContentType,
                FileSizeInBytes = d.FileSizeInBytes,
                UploadedAtUtc = d.UploadedAtUtc
            })
            .ToListAsync();

        return Ok(documents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetDocumentById(int id)
    {
        DocumentDto? document = await dbContext.Documents
            .Where(d => d.Id == id)
            .Select(d => new DocumentDto
            {
                Id = d.Id,
                OriginalFilename = d.OriginalFilename,
                StoredFilename = d.StoredFilename,
                ContentType = d.ContentType,
                FileSizeInBytes = d.FileSizeInBytes,
                UploadedAtUtc = d.UploadedAtUtc
            })
            .SingleOrDefaultAsync();

        if(document is null)
        {
            return NotFound();
        }

        return Ok(document);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(FileHelperService fileHelper, int id)
    {
        Document? document = await dbContext.Documents
            .Where(d => d.Id == id)
            .SingleOrDefaultAsync();

        if(document is null)
        {
            return NotFound();
        }

        var fileBytes = await fileHelper.GetFileBytesFromStorageAsync(document.StoredFilename);

        if(fileBytes is null)
        {
            return NotFound();
        }

        return File(
        fileBytes,
        document.ContentType,
        document.OriginalFilename);
    }

    [HttpPost]
    public async Task<ActionResult> UploadDocument(FileHelperService fileHelper, IFormFile file)
    {
        if (!FileHelperService.IsFileValid(file))
        {
            return Problem(
                title: "Bad Request",
                detail: "Upload failed. Please check file and try again.",
                statusCode: StatusCodes.Status400BadRequest
            );
        }
        
        if(file.FileName is not null)
        {
            bool isFilenameExists = dbContext.Documents
                .Any(d => d.OriginalFilename == file.FileName);    

            if(isFilenameExists)
            {
                return Problem(
                    title: "Conflict",
                    detail: "A file with the same name already exists. Please rename the file and try again.",
                    statusCode: StatusCodes.Status409Conflict
                );
            }
        }

        string generatedFilename = await fileHelper.SaveToStorageAsync(file);

        var document = new Document{
            OriginalFilename = file.FileName!,
            StoredFilename = generatedFilename,
            FileSizeInBytes = file.Length,
            ContentType = file.ContentType,
            UploadedAtUtc = DateTime.UtcNow
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDocumentById), new { id = document.Id}, document);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDocumentById(FileHelperService fileHelper, int id)
    {
        Document? document = await dbContext.Documents
            .Where(d => d.Id == id)
            .SingleOrDefaultAsync();

        if(document is null)
        {
            return NotFound();
        }

        fileHelper.DeleteFromStorage(document.StoredFilename);

        dbContext.Documents.Remove(document);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}