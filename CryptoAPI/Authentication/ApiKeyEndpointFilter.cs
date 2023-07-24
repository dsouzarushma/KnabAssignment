namespace CryptoAPI.Authentication
{
    public class APIKeyEndpointFilter : IEndpointFilter
    {
        private readonly IConfiguration configuration;
        public APIKeyEndpointFilter(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                return TypedResults.Unauthorized();
            }
            var apiKey=configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (!apiKey.Equals(extractedApiKey))
            { 
                return TypedResults.Unauthorized();
            }
            return await next(context);
        }
    }
}
