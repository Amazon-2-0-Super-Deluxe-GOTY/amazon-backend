using amazon_backend.CQRS.Queries.Request;
using amazon_backend.Data;
using amazon_backend.Data.Dao;
using amazon_backend.Data.Entity;
using amazon_backend.Data.Model;
using amazon_backend.Profiles.ProductProfiles;
using amazon_backend.Services.FluentValidation;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace amazon_backend.Controllers
{
    [Route("api/filter")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IValidator<GetFilterItemsQueryRequest> getFiltItemsValidator;
        public FilterController(IValidator<GetFilterItemsQueryRequest> getFiltItemsValidator, IMediator mediator)
        {
            this.getFiltItemsValidator = getFiltItemsValidator;
            this.mediator = mediator;
        }
        [HttpGet]
        [Route("filter-items")]
        public async Task<IActionResult> GetFilterItems([FromQuery]GetFilterItemsQueryRequest request)
        {
            var validationErrors = getFiltItemsValidator.GetErrors(request);
            if (validationErrors != null)
            {
                return SendResponse(StatusCodes.Status400BadRequest, "Bad request", validationErrors);
            }
            var response = await mediator.Send(request);
            if (!response.isSuccess)
            {
                return SendResponse(StatusCodes.Status404NotFound, response.message, null);
            }
            return SendResponse(StatusCodes.Status200OK, "Ok", response.data);
        }
       
        private IActionResult SendResponse(int statusCode, string message, object data, string contentType = "application/json")
        {
            HttpContext.Response.StatusCode = statusCode;
            HttpContext.Response.ContentType = contentType;
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = statusCode,
                    Message = message,
                    Data = data
                }, settings));
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"error json: {ex.Message}");
                return Content(JsonConvert.SerializeObject(new
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "See server logs",
                    Data = (object)null!
                }, settings));
            }
        }
    }
}
