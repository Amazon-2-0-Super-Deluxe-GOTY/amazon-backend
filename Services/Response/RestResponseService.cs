using amazon_backend.Services.Response.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace amazon_backend.Services.Response
{
    public class RestResponseService
    {
        private readonly ILogger<RestResponseService> _logger;
        public JsonSerializerSettings jsonSerializerSettings { get; set; }
        public RestResponseService(ILogger<RestResponseService> logger)
        {
            _logger = logger;
        }
        public string contentType { get; set; } = "application/json";
        public IActionResult SendResponse(HttpContext context,int statusCode,string message, object data,Count count=null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;
            try
            {
                object response;
                if (count != null)
                {
                    response = new
                    {
                        Status = statusCode,
                        Message = message,
                        Count = count,
                        Data = data
                    };
                }
                else
                {
                    response = new
                    {
                        Status = statusCode,
                        Message = message,
                        Data = data
                    };
                }
                var serializeResult = SerializeObject(response);
                return new ContentResult()
                {
                    Content = serializeResult,
                    StatusCode = statusCode,
                    ContentType = contentType
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex.Message);
                return new ContentResult()
                {
                    Content = "Server error. See server logs",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ContentType = contentType
                };
            }
            catch(SerializerSettingsException ex)
            {
                _logger.LogError(ex.Message);
                return new ContentResult()
                {
                    Content = "Server error. See server logs",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ContentType = contentType
                };
            }
        }
        private string SerializeObject(object data)
        {
            if (jsonSerializerSettings is null)
            {
                throw new SerializerSettingsException("Serializer settings not specified");
            }
            return JsonConvert.SerializeObject(data, jsonSerializerSettings);
        }

    }
    public class SerializerSettingsException : ApplicationException
    {
        public SerializerSettingsException(string message) : base(message) { }
    }
}
