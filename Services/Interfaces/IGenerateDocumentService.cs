using BizCover.Common.DtoModels.Certificate;

namespace BizCover.Utility.Document.Template.Services.Interfaces
{
    public interface IGenerateDocumentService
    {
        string GenerateCertificate(Certificate certificate);
    }
}