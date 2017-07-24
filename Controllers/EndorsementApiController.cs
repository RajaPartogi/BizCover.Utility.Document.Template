using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BizCover.Common.DtoModels.Endorsement;
using BizCover.Utility.Document.Template.Services.Interfaces;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class EndorsementApiController : BaseApiController
    {
        private readonly IGenerateDocumentService _generateDocumentService;
        private readonly IFileService _fileService;

        public EndorsementApiController(IGenerateDocumentService generateDocumentService, IFileService fileService)
        {
            _generateDocumentService = generateDocumentService;
            _fileService = fileService;
        }

        [HttpPost]
        [Route("api/getEndorsement")]
        public HttpResponseMessage GetEndorsement([FromBody] EndorsementDto endorsement)
        {   
            if (endorsement == null)
                return ReturnErrorResponseMessage(HttpStatusCode.BadRequest, "Ceritificate payload is invalid");

            try
            {
                var EndorsementPath = _generateDocumentService.GenerateEndorsement(endorsement);
                if (string.IsNullOrEmpty(EndorsementPath))
                    return ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, "Endorsement generation failed");

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;

                if (endorsement.ParseEmptyText)
                    return response;

                var responseStream = _fileService.GetMemoryStream(EndorsementPath);
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
