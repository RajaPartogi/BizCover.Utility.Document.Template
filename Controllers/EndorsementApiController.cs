using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BizCover.Common.DtoModels.Endorsement;
using BizCover.Utility.Document.Template.Services.Interfaces;
using BizCover.Common.Infrastructure.Logging;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class EndorsementApiController : BaseApiController
    {
        private readonly IGenerateDocumentService _generateDocumentService;
        private readonly IFileService _fileService;
        private readonly ILogger _logger;
        
        public EndorsementApiController(IGenerateDocumentService generateDocumentService, IFileService fileService, ILogger logger)
        {
            _generateDocumentService = generateDocumentService;
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/getEndorsement")]
        public HttpResponseMessage GetEndorsement([FromBody] EndorsementDto endorsement)
        {
            if (endorsement == null)
            {
                return ReturnErrorResponseMessage(HttpStatusCode.BadRequest, "Endorsement payload is invalid");
            }

            try
            {
                _logger.LogTrace("GetEndorsement :: GetPath started.");
                var endorsementPath = _generateDocumentService.GenerateEndorsement(endorsement);
                _logger.LogTrace("GetEndorsement :: GetPath done. :: EndorsementPath :: " + endorsementPath);

                if (string.IsNullOrWhiteSpace(endorsementPath))
                {
                    _logger.LogTrace("GetEndorsement :: EndorsementPath is null or empty.");
                    return ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, "Endorsement generation failed :: TemplateUrl: " + endorsement.TemplatePdfUrl + " :: EmptyParse: " + endorsement.ParseEmptyText);
                }

                if (_fileService.IsValidPdf(endorsementPath) == false)
                {
                    _logger.LogTrace("GetEndorsement :: EndorsementPath is invalid pdf::" + endorsementPath);
                    return ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, "Endorsement generation failed :: invalid pdf :: TemplateUrl: " + endorsement.TemplatePdfUrl + " :: EmptyParse: " + endorsement.ParseEmptyText);
                }

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                };

                _logger.LogTrace("GetEndorsement :: Get memomry stream from endorsment at ." + endorsementPath);

                var responseStream = _fileService.GetMemoryStream(endorsementPath);

                _logger.LogTrace("GetEndorsement :: Get memomry stream done .");

                response.Content = new StreamContent(responseStream);

                return response;
            }
            catch (IOException ex)
            {
                _logger.LogException(ex);
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message + " :: TemplateUrl: " + endorsement.TemplatePdfUrl);
                throw new HttpResponseException(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message + " :: TemplateUrl: " + endorsement.TemplatePdfUrl);
                throw new HttpResponseException(errorResponse);
            }
        }
    }
}
