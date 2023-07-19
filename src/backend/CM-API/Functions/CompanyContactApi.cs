using DataLayer.Interfaces;
using FunctionDI.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FunctionDI.Functions
{
    public sealed class CompanyContactAPI
    {
        private readonly ILogger _logger;
        private readonly ICompanyContactHandler _companyContactHandler;

        public CompanyContactAPI(ILoggerFactory loggerFactory, ICompanyContactHandler companyContactHandler)
        {
            _logger = loggerFactory.CreateLogger<CompanyContactAPI>();
            _companyContactHandler = companyContactHandler;
        }

        [Function("CompanyContact-Get")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiParameter("orderby", Required = false, Description = "Property to order by")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CompanyContactView[]), Description = "The OK response")]
        public async Task<HttpResponseData> GetList([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companyContacts/{orderby?}/{asc:bool?}")] HttpRequestData req, string orderby, bool? asc)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            CompanyContactView[] items = await _companyContactHandler.Get(orderby, (asc.HasValue)? asc.Value: true);
            return await req.CreateOkJsonResponse(items);
        }

        [Function("CompanyContact-GetById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiParameter("id", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CompanyContact), Description = "The OK response")]
        public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companyContact/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var item = await _companyContactHandler.GetById(id);
            return await req.CreateOkJsonResponse(item);
        }

        [Function("CompanyContact-GetByCompanyId")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiParameter("id", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<CompanyContact>), Description = "The OK response")]
        public async Task<HttpResponseData> GetByCompanyId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companyContactsForCompany/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var item = await _companyContactHandler.GetByCompanyId(id);
            return await req.CreateOkJsonResponse(item);
        }

        [Function("CompanyContact-Add")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiRequestBody("application/json", typeof(CompanyContact),
            Description = "JSON request body containing CompanyContact item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companyContact")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var companyContact = JsonConvert.DeserializeObject<CompanyContact>(request);
            companyContact.Id = Guid.NewGuid().ToString();
            await _companyContactHandler.Add(companyContact);

            return req.CreateOkTextResponse($"Created with Id: {companyContact.Id}");
        }

        [Function("CompanyContact-Update")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiRequestBody("application/json", typeof(CompanyContact),
            Description = "JSON request body containing updated CompanyContact item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companyContact")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var companyContact = JsonConvert.DeserializeObject<CompanyContact>(request);
            await _companyContactHandler.Update(companyContact);

            return req.CreateOkTextResponse($"Updated id: {companyContact.Id}");
        }

        [Function("CompanyContact-DeleteById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "CompanyContact" })]
        [OpenApiParameter("id", Required = true, Description = "ID of CompanyContact item to remove")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> DeleteById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companyContact/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _companyContactHandler.Delete(id);
            return req.CreateOkTextResponse($"Deleted id: {id}");
        }
    }
}
