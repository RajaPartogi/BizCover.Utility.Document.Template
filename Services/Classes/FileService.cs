using System;
using System.IO;
using BizCover.Common.Constants;
using BizCover.Utility.Document.Template.Constants;
using BizCover.Utility.Document.Template.Services.Interfaces;

namespace BizCover.Utility.Document.Template.Services
{
    public class FileService : IFileService
    {
        public MemoryStream GetMemoryStream(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var memoryStream = new MemoryStream();
            Stream fileStream = File.Open(filePath, FileMode.Open);
            fileStream.CopyTo(memoryStream);
            fileStream.Close();
            memoryStream.Position = 0;

            return memoryStream;
        }

        public string GetFileName(string perfix, int applicationId, int productId)
        {
            var timestamp = DateTime.Now.ToString(BizCoverConstants.S_TIMESTAMP_DATEFORMAT);
            var filename = string.Format(CertificateConstant.S_DOCUMENT_FILENAME_FORMAT, applicationId, productId, timestamp);
            if (string.IsNullOrEmpty(perfix) == false)
                filename = perfix + "_" + filename;

            return filename;
        }
    }
}