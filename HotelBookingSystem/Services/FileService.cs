public class FileService : IFileServices
{
    private readonly IWebHostEnvironment _Host;

    public FileService(IWebHostEnvironment Host)
    {
        _Host = Host;
    }

    public void DeleteFile(string Folderpath, string fileNamewithExtension)
    {
        var path = Path.Combine(_Host.WebRootPath, Folderpath, Path.GetFileName(fileNamewithExtension));
        var IsExist = Path.Exists(path);
        if (IsExist)
        {
            File.Delete(path);
        }
    }

    public string SaveFile(IFormFile file, string FolderPath)
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
        var FileUrl = "";
        string fileName = Guid.NewGuid().ToString();
        string extension = Path.GetExtension(file.FileName);
        using (FileStream fileStreams = new(Path.Combine(FolderPath,
                        fileName + extension), FileMode.Create))
        {
            file.CopyTo(fileStreams);
        }

        FileUrl = fileName + extension;
          return FileUrl;
    }
}
public interface IFileServices
{

    public void DeleteFile(string Folderpath, string fileNamewithExtension);
    public string SaveFile(IFormFile file, string FolderPath);
}