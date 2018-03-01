using System;
using System.IO;
using BizCover.Common.Constants;
using BizCover.Common.Infrastructure.Logging;
using BizCover.Utility.Document.Template.Constants;
using BizCover.Utility.Document.Template.Services.Interfaces;
using iTextSharp.text.pdf;

namespace BizCover.Utility.Document.Template.Services.Classes
{
    public class FileService : IFileService
    {
        private readonly ILogger _logger;

        public FileService(ILogger logger)
        {
            _logger = logger;
        }

        public MemoryStream GetMemoryStream(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

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
            {
                filename = perfix + "_" + filename;
            }

            return filename;
        }

        public bool IsValidPdf(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                return false;
            }

            var result = false;     
            
            try
            {
                using (var reader = new PdfReader(filepath))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                File.Delete(filepath);
            }            

            return result;
        }
    }
}