using Microsoft.AspNetCore.Http;

namespace SDMS.Test.Helpers;

public static class FileHelper
{
    private const long OneMB = 1024 * 1024;
    
    public static FormFile CreateTestFile(
        string fileName,
        string contentType,
        long sizeInBytes
    )
    {
        var content = new byte[sizeInBytes];
        var stream = new MemoryStream(content);

        return new FormFile(
            stream,
            0,
            stream.Length,
            "file",
            fileName
        )
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    public static long ConvertMBToBytes(long sizeInMB) => sizeInMB * OneMB;
}