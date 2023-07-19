using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace FunctionDI.Extensions
{
    public static class HttpRequestDataExtensions
    {
        public static HttpResponseData CreateOkTextResponse(this HttpRequestData req, string message)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(message);

            return response;
        }

        public static HttpResponseData CreateBadRequestTextResponse(this HttpRequestData req, string message)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(message);

            return response;
        }

        public static async Task<HttpResponseData> CreateOkJsonResponse(this HttpRequestData req, object item)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(item);

            return response;
        }
    }
}
