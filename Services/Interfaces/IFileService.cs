using System.IO;

namespace BizCover.Utility.Document.Template.Services.Interfaces
{
    public interface IFileService
    {
        MemoryStream GetMemoryStream(string filePath);
        string GetFileName(string perfix, int applicationId, int productId);
    }
}