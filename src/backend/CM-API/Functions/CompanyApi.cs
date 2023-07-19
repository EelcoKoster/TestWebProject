using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer.Interfaces;
using FunctionDI.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace FunctionDI.Functions
{
    public sealed class CompanyAPI
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICompanyHandler _companyHandler;

        public CompanyAPI(ILoggerFactory loggerFactory, IConfiguration config, ICompanyHandler companyHandler)
        {
            _logger = loggerFactory.CreateLogger<CompanyAPI>();
            _configuration = config;
            _companyHandler = companyHandler;
        }

        [Function("Company-Get")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Company" })]
        [OpenApiParameter("orderby", Required = false, Description = "Property to order by")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Company>), Description = "The OK response")]
        public async Task<HttpResponseData> GetList([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{orderby?}/{asc:bool?}")] HttpRequestData req, string orderby, bool? asc)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var items = _companyHandler.Get(orderby, asc);
            return await req.CreateOkJsonResponse(items.ToArray());
        }

        [Function("Company-GetById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Company" })]
        [OpenApiParameter("id", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "company/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var item = await _companyHandler.GetById(id);
            return await req.CreateOkJsonResponse(item);
        }

        [Function("Company-Add")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Company" })]
        [OpenApiRequestBody("application/json", typeof(Company),
            Description = "JSON request body containing Company item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "company")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var company = JsonConvert.DeserializeObject<Company>(request);
            company.Id = Guid.NewGuid().ToString();
            await _companyHandler.Add(company);

            return req.CreateOkTextResponse($"Created with Id: {company.Id}");
        }

        [Function("Company-Update")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Company" })]
        [OpenApiRequestBody("application/json", typeof(Company),
            Description = "JSON request body containing updated Company item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "company")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var company = JsonConvert.DeserializeObject<Company>(request);
            await _companyHandler.Update(company);

            return req.CreateOkTextResponse($"Updated id: {company.Id}");
        }

        [Function("Company-DeleteById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Company" })]
        [OpenApiParameter("id", Required = true, Description = "ID of Company item to remove")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> DeleteById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "company/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _companyHandler.Delete(id);
            return req.CreateOkTextResponse($"Deleted id: {id}");
        }
    }
}
