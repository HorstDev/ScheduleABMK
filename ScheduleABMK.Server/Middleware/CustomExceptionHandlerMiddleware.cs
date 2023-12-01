using System.Net;
using System.Text.Json;

namespace ScheduleABMK.Server.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next) =>
            _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch(exception)
            {
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case BadHttpRequestException:
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            result = JsonSerializer.Serialize(new { error = exception.Message });

            return context.Response.WriteAsync(result);
        }
    }
}
