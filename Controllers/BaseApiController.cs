using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BizCover.Utility.Document.Template.Controllers
{
    public class BaseApiController : ApiController
    {
        public NameValueCollection GetCollectionQueryString()
        {
            var currurl = HttpContext.Current.Request.RawUrl;
            var querystring = string.Empty;

            // Check to make sure some query string variables
            int iqs = currurl.IndexOf('?');
            if (iqs == -1)
                return null;

            if (iqs >= 0)
                querystring = (iqs < currurl.Length - 1) ? currurl.Substring(iqs + 1) : String.Empty;

            // Parse the query string variables into a NameValueCollection.
            if (string.IsNullOrEmpty(querystring))
                return null;

            return HttpUtility.ParseQueryString(querystring);
        }

        protected HttpResponseMessage ReturnErrorResponseMessage(HttpStatusCode statusCode, string errorMessage)
        {
            var errorResponse = new HttpResponseMessage(statusCode);
            errorResponse.Content = new StringContent(errorMessage);
            return errorResponse;
        }
    }
}