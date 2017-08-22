using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BizCover.Common.DtoModels.Endorsement;
using BizCover.Utility.Document.Template.Services.Classes;
using BizCover.Utility.Document.Template.Services.Interfaces;
using log4net;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class EndorsementApiController : BaseApiController
    {
        private readonly IGenerateDocumentService _generateDocumentService;
        private readonly IFileService _fileService;
        private readonly DocumentTemplateLogger _logger;
        
        public EndorsementApiController(IGenerateDocumentService generateDocumentService, IFileService fileService)
        {
            _generateDocumentService = generateDocumentService;
            _fileService = fileService;
            _logger = new DocumentTemplateLogger();
        }

        [HttpPost]
        [Route("api/getEndorsement")]
        public HttpResponseMessage GetEndorsement([FromBody] EndorsementDto endorsement)
        {   
            if (endorsement == null)
                return ReturnErrorResponseMessage(HttpStatusCode.BadRequest, "Endorsement payload is invalid");

            try
            {
                _logger.WriteInfo("GetEndorsement :: GetPath started.");
                var EndorsementPath = _generateDocumentService.GenerateEndorsement(endorsement);
                _logger.WriteInfo("GetEndorsement :: GetPath done. :: EndorsementPath :: " + EndorsementPath);

                if (string.IsNullOrEmpty(EndorsementPath))
                {
                    _logger.WriteInfo("GetEndorsement :: EndorsementPath is null or empty.");
                    return ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, "Endorsement generation failed :: TemplateUrl: " + endorsement.TemplatePdfUrl + " :: EmptyParse: " + endorsement.ParseEmptyText);
                }

                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.OK;

                _logger.WriteInfo("GetEndorsement :: Get memomry stream from endorsment at ." + EndorsementPath);

                var responseStream = _fileService.GetMemoryStream(EndorsementPath);

                _logger.WriteInfo("GetEndorsement :: Get memomry stream done .");

                response.Content = new StreamContent(responseStream);

                return response;
            }
            catch (IOException ex)
            {
                _logger.WriteError("GetEndorsement :: IOException occured. Message: " + ex.Message, ex);
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message + " :: TemplateUrl: " + endorsement.TemplatePdfUrl);
                throw new HttpResponseException(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.WriteError("GetEndorsement :: Exception occured. Message: " + ex.Message, ex);
                var errorResponse = ReturnErrorResponseMessage(HttpStatusCode.InternalServerError, ex.Message + "::InnerException::" + ex.InnerException?.Message + " :: TemplateUrl: " + endorsement.TemplatePdfUrl);
                throw new HttpResponseException(errorResponse);
            }
        }
    }
}
