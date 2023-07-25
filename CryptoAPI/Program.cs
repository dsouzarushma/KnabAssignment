
using CryptoAPI.Authentication;
using CryptoAPI.Models;
using CryptoAPI.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using static CryptoAPI.Models.CoinAPIResponse;

namespace CryptoAPI
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x=>
            {
                x.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description="This is API key to access the API",
                    Type=SecuritySchemeType.ApiKey,
                    Name= "x-api-key",
                    In=ParameterLocation.Header,
                    Scheme="ApiKeyScheme"
                });
                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header,
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    { scheme,new List<string>()}
                };
                x.AddSecurityRequirement(requirement);
            });
            builder.Services.AddHttpClient();

            //Enable cors
            builder.Services.AddCors(p => p.AddDefaultPolicy(build => {
                build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
            }));

            //Enable rate limiting
            builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", option =>
            {
                option.Window = TimeSpan.FromSeconds(10);
                option.PermitLimit = 1;
                option.QueueLimit = 1;
                option.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            }));

            var coinMarketConfiguration = builder.Configuration.GetSection("CoinMarketAPISettings").Get<APIConfiguration>();
            builder.Services.AddHttpClient<CoinMarketHttpClient>(client =>
            {
                client.DefaultRequestHeaders.Add(coinMarketConfiguration.apiKeyName, coinMarketConfiguration.apiKeyValue);
                client.BaseAddress = new Uri(coinMarketConfiguration.apiURL);
            });
            var exchangeRateConfiguration = builder.Configuration.GetSection("ExchangeRateAPISettings").Get<APIConfiguration>();
            builder.Services.AddHttpClient<ExchangesRateHttpClient>(client =>
            {
                client.BaseAddress = new Uri(exchangeRateConfiguration.apiURL);
            });

            var logPath = builder.Configuration.GetSection("Logging:LogPath").Value;
            var _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(logPath)
                .CreateLogger();
            builder.Logging.AddSerilog(_logger);

            var app = builder.Build();
            app.UseRateLimiter();
           
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();       
                app.UseSwaggerUI();
            }
            app.UseCors();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapCryptoAPIEndPoints(builder);

            app.Run();
            
        }
       
    }
    public partial class Program { }
}