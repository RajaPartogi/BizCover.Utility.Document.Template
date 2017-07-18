using BizCover.Common.DtoModels.Certificate;
using BizCover.Common.DtoModels.Endorsement;

namespace BizCover.Utility.Document.Template.Services.Interfaces
{
    public interface IGenerateDocumentService
    {
        string GenerateCertificate(Certificate certificate);
        string GenerateEndorsement(EndorsementDto endorsement);
    }
}