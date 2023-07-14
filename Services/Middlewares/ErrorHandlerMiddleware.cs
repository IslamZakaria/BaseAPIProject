using DTOs.Shared.Exceptions;
using DTOs.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MOCA.Services
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message 
                };

                switch (error)
                {
                    case ApiException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Errors = new List<string> { e.Message };
                        break;

                    case ExistsBeforeException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.Created;
                        responseModel.Errors = new List<string> { e.Message };
                        break;
                    case EntityIsBusyException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        responseModel.Errors = new List<string> { e.Message };
                        break;
                    case NotFoundException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.Errors = new List<string> { e.Message };
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case DbUpdateException e:
                        response.StatusCode = (int)HttpStatusCode.FailedDependency;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
        }
    }
}
