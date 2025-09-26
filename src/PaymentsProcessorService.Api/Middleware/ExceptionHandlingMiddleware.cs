namespace PaymentsProcessorService.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var result = new
            {
                error = "An internal server error occurred.",
                detalhe = ex.Message
            };

            await context.Response.WriteAsJsonAsync(result);
        }
    }
}