using System;
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
                return ReturnErrorResponseMessage(HttpStatusCode.BadRequest, "Ceritificate payload is invalid");

            try
            {
                var certificatePath = _generateDocumentService.GenerateCertificate(certificate);
                if (string.IsNullOrEmpty(certificatePath))
                    return ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, "Certificate generation failed");

                var responseStream = _fileService.GetMemoryStream(certificatePath);

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StreamContent(responseStream);
                return response;
            }
            catch (IOException ex)
            {
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message);
                throw new HttpResponseException(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message);
                throw new HttpResponseException(errorResponse);
            }
        }
    }
}
