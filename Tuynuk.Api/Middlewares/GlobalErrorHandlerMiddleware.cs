using Newtonsoft.Json;
using System.Net;
using Tuynuk.Infrastructure.Response;

namespace Tuynuk.Api.Middlewares
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalErrorHandlerMiddleware> logger;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, logger, context);
            }
        }

        public Task HandleExceptionAsync(Exception ex, ILogger logger, HttpContext context)
        {
            var error = new BaseError();

            if (ex.InnerException != null)
            {
                error.Message = ex.InnerException.Message;
            }
            else
            {
                error.Message = ex.Message;
            }

            logger.LogError(ex.Message, ex);

            var response = new BaseResponse
            {
                Error = error
            };

            string baseResponseJson = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(baseResponseJson);
        }

    }
}
