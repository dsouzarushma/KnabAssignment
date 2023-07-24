
using CryptoAPI.Authentication;
using CryptoAPI.Models;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using static CryptoAPI.Models.CoinAPIResponse;

namespace CryptoAPI
{
    public class Program
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

            builder.Services.AddCors(p => p.AddDefaultPolicy(build => {
                build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
            }));
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
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("X-CMC_PRO_API_KEY", "97f0e6e0-fd89-4571-bb72-ed5694932839");
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

            app.MapGet("/getQuote", async (string currencyCode, CoinMarketHttpClient coinMarketHttpClient, ExchangesRateHttpClient exchangesRateHttpClient) =>
            {
                app.Logger.LogInformation("Call API start");
                //Read configuration details from appSettings
                string baseCurrency = builder.Configuration.GetValue<string>("BaseCurrency");
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                string exchangeCurrencies = builder.Configuration.GetValue<string>("ExchangeCurrencies");
                if (currencyCode.Length > 3)
                {
                    return Results.BadRequest("Invalid Crypto Currency code");
                }
                //Call CoinMarketAPI to get Quote for CryptoCurrency in EUR
                app.Logger.LogInformation("Call Coin market API");
                var coinMarketResponse = await coinMarketHttpClient.GetCryptoQuote(currencyCode, baseCurrency);
                app.Logger.LogInformation("Coin market API call complete.");
                if (coinMarketResponse.status.error_code != 0)
                {
                    app.Logger.LogInformation("Coin market API call returned error response.");
                    return (coinMarketResponse.status.error_message);
                }
                app.Logger.LogInformation("Coin market API call returned response:" + coinMarketResponse);
                float basePrice = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].price;

                //Add EUR rate to response collection
                Dictionary<string, Currency> finalResponse = new Dictionary<string, Currency>
                {
                    { baseCurrency, coinMarketResponse.data[currencyCode][0].quote[baseCurrency] }
                };

                //Call ExcangeRate API to get rates for remaining currencies
                app.Logger.LogInformation("Exchange market API");
                var exchangeResponse = await exchangesRateHttpClient.GetExchangeRate(baseCurrency, exchangeCurrencies);
                app.Logger.LogInformation("Exchange API call complete.");
                if (!exchangeResponse.success)
                {
                    app.Logger.LogInformation("Exchange API call returned error response.");
                    httpResponseMessage.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                    return (httpResponseMessage);
                }
                app.Logger.LogInformation("Exchange API call returned response:" + exchangeResponse);
                //Add all other currencies and calculated rate to response collection
                foreach (var currency in exchangeResponse.rates)
                {
                    Currency currencyQuotes = new Currency {
                        price = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].price * currency.Value,
                        volume_24h = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].volume_24h,
                        volume_change_24h = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].volume_change_24h,
                        percent_change_1h = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_1h,
                        percent_change_24h = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_24h,
                        percent_change_7d = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_7d,
                        percent_change_30d = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_30d,
                        percent_change_60d = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_60d,
                        percent_change_90d = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].percent_change_90d,
                        market_cap = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].market_cap,
                        market_cap_dominance = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].market_cap_dominance,
                        fully_diluted_market_cap = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].fully_diluted_market_cap,
                        tvl = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].tvl,
                        last_updated = coinMarketResponse.data[currencyCode][0].quote[baseCurrency].last_updated
                    };
                    finalResponse.Add(currency.Key, currencyQuotes);
                }
                app.Logger.LogInformation("API final response:" + finalResponse);
                return finalResponse;
            })
          .WithName("GetQuote")
          .WithOpenApi(operation =>
          {
              var parameter = operation.Parameters[0];
              parameter.Description = "Enter Crypto Currency Code";
              operation.Summary = "This API is used to get quote for user provided Cryptocurrency in USD,EUR,GBP,BRL and AUD";
              operation.Description = "This API internally calls 2 external APIs to get the latest quotes and to convert the rate to desired currencies.";
              return operation;              
          })
          .AddEndpointFilter<APIKeyEndpointFilter>()
          .WithMetadata(new EnableRateLimitingAttribute("fixedwindow")).Produces<Dictionary<string, Currency>>(200).Produces(400).Produces(500);
                
            app.Run();
        }
    }
}