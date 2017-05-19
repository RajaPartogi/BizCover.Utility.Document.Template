using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BizCover.Common.DtoModels.Certificate;
using BizCover.Utility.Document.Template.Services.Interfaces;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class CertificateApiController : BaseApiController
    {
        private readonly IGenerateDocumentService _generateDocumentService;
        private readonly IFileService _fileService;

        public CertificateApiController(IGenerateDocumentService generateDocumentService, IFileService fileService)
        {
            _generateDocumentService = generateDocumentService;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("api/getcertificate")]
        public HttpResponseMessage GetCertificate([FromBody] Certificate certificate)
        {   
            if (certificate == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var certificatePath = _generateDocumentService.GenerateCertificate(certificate);
            if (string.IsNullOrEmpty(certificatePath))
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                var responseStream = _fileService.GetMemoryStream(certificatePath);

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StreamContent(responseStream);
                return response;
            }
            catch (IOException ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
