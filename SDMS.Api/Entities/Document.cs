namespace SDMS.Entities;

public sealed class Document
{
    public int Id {get;init;}
    public required string OriginalFilename {get; init;}
    public required string StoredFilename {get; init;}
    public required string ContentType {get;init;}
    public required long FileSizeInBytes {get;init;}
    public required DateTime UploadedAtUtc {get;init;}
}