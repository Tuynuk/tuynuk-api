using Tuynuk.Api.Middlewares;

namespace Tuynuk.Api.Extensions
{
    public static class ApplicationBuilderExtentions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<GlobalErrorHandlerMiddleware>();
        }
    }
}
