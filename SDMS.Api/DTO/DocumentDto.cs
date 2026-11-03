namespace SDMS.DTO;

public sealed record DocumentDto
{
    public required int Id {get;init;}
    public required string OriginalFilename {get; init;}
    public required string StoredFilename {get; init;}
    public required string ContentType {get;init;}
    public required long FileSizeInBytes {get;init;}
    public required DateTime UploadedAtUtc {get;init;}
}