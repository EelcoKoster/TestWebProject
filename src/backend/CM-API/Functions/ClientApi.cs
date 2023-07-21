using DataLayer.Interfaces;
using FunctionDI.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FunctionDI.Functions
{
    public sealed class ClientApi
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IClientHandler _clientHandler;

        public ClientApi(ILoggerFactory loggerFactory, IConfiguration config, IClientHandler clientHandler)
        {
            _logger = loggerFactory.CreateLogger<ClientApi>();
            //_logger.LogInformation("ClientApi constructor called.");
            _configuration = config;
            _clientHandler = clientHandler;
        }

        [Function("Client-Get")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Client" })]
        [OpenApiParameter("orderby", Required = false, Description = "Property to order by")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ClientView[]), Description = "The OK response")]
        public async Task<HttpResponseData> GetList([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "clients/{orderby?}/{asc:bool?}")] HttpRequestData req, string orderby, bool? asc)
        {
            _logger.LogInformation("Get clients request.");
            ClientView[] items = await _clientHandler.Get(orderby, asc);
            return await req.CreateOkJsonResponse(items);
        }

        [Function("Client-GetById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Client" })]
        [OpenApiParameter("id", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Client), Description = "The OK response")]
        public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "client/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Get client with id '{id}' request.");
            var item = await _clientHandler.GetById(id);
            return await req.CreateOkJsonResponse(item);
        }

        [Function("Client-Add")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Client" })]
        [OpenApiRequestBody("application/json", typeof(Client), Description = "JSON request body containing Client item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The validation error response")]
        public async Task<HttpResponseData> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "client")] HttpRequestData req)
        {
            _logger.LogInformation("Add client request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var client = JsonConvert.DeserializeObject<Client>(request);
            var validator = new ClientValidator();
            var results = validator.Validate(client);
            if (!results.IsValid) return req.CreateBadRequestTextResponse(results.ToString(", "));

            client.Id = Guid.NewGuid().ToString();
            await _clientHandler.Add(client);

            return req.CreateOkTextResponse($"Created with Id: {client.Id}");
        }

        [Function("Client-Update")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Client" })]
        [OpenApiRequestBody("application/json", typeof(Client), Description = "JSON request body containing updated Client item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "The validation error response")]
        public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "client")] HttpRequestData req)
        {
            _logger.LogInformation($"Update client request.");

            var request = await new StreamReader(req.Body).ReadToEndAsync();
            var client = JsonConvert.DeserializeObject<Client>(request);
            var validator = new ClientValidator();
            var results = validator.Validate(client);
            if (!results.IsValid) return req.CreateBadRequestTextResponse(results.ToString(", "));

            await _clientHandler.Update(client);

            return req.CreateOkTextResponse($"Updated id: {client.Id}");
        }

        [Function("Client-DeleteById")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Client" })]
        [OpenApiParameter("id", Required = true, Description = "ID of Client item to remove")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<HttpResponseData> DeleteById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "client/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Delete client with id '{id}' request.");
            await _clientHandler.Delete(id);
            return req.CreateOkTextResponse($"Deleted id: {id}");
        }
    }
}
