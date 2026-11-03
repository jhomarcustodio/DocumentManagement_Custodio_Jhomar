using System.Diagnostics;

namespace SDMS.Services;

public sealed class FileHelperService
{

    private readonly string _uploadsDirectory;

    public FileHelperService()
    {
        _uploadsDirectory = GetUploadsDirectory();
    }

    public async Task<string> SaveToStorageAsync(IFormFile file)
    {
        try
        {
            var (fileName, filePath) = GetFileNameAndPath(file.FileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await  file.CopyToAsync(stream);

            return fileName;
        }
        catch
        {
            throw;
        }
    }

    public async Task<byte[]?> GetFileBytesFromStorageAsync(string filename)
    {
        try
        {
            var (_,filePath) = GetFileNameAndPath(filename, true);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            return fileBytes;
        }
        catch
        {
            throw;
        }
    }

    public bool? DeleteFromStorage(string filename)
    {
        try
        {
            var (_,filePath) = GetFileNameAndPath(filename, true);

            if (!File.Exists(filePath))
            {
                return null;
            }

            File.Delete(filePath);

            return true;
        }
        catch
        {
            throw;
        }

    }

    public static bool IsFileValid(IFormFile file)
    {
        string[] acceptedMimes = ["application/pdf"];
        const long maxFileSize = 10 * 1024 * 1024;

        if(file is null ||
            string.IsNullOrEmpty(file.FileName) ||
            file.Length == 0 ||
            !acceptedMimes.Contains(file.ContentType) || 
            file.Length > maxFileSize
        )
        {
            return false;
        }

        return true;
    }

    private (string,string) GetFileNameAndPath(string filename, bool useProvidedFilename = false)
    {
        try
        {
            string fileName = useProvidedFilename ? filename : $"{Guid.NewGuid()}{Path.GetExtension(filename)}";
            string filePath = Path.Combine(_uploadsDirectory, fileName);

            return (fileName, filePath);
        }
        catch
        {
            throw;
        }
    }

    private string GetUploadsDirectory()
    {
        try
        {
            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads"
            );

            Directory.CreateDirectory(uploadsFolder);

            return uploadsFolder;
        }
        catch
        {
            throw;
        }
    }

}