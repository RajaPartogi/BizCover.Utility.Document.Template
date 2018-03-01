using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class BaseApiController : ApiController
    {
        protected HttpResponseMessage ReturnErrorResponseMessage(HttpStatusCode statusCode, string errorMessage)
        {
            var errorResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage)
            };

            return errorResponse;
        }
    }
}