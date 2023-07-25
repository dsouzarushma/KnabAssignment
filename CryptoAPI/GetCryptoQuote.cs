using CryptoAPI.Authentication;
using CryptoAPI.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using static CryptoAPI.Models.CoinAPIResponse;

namespace CryptoAPI
{
    public static class GetCryptoQuote
    {
        public static void MapCryptoAPIEndPoints(this WebApplication app,WebApplicationBuilder builder)
        {
            app.MapGet("/getQuote/{currencyCode}", async (string currencyCode, CoinMarketHttpClient coinMarketHttpClient, ExchangesRateHttpClient exchangesRateHttpClient) =>
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
                    app.Logger.LogInformation("Coin market API call returned error response.ErrorCode:"+coinMarketResponse.status.error_code+"ErrorMessage:"+ coinMarketResponse.status.error_message);
                    return (coinMarketResponse.status.error_message);
                }
                app.Logger.LogInformation("Coin market API call returned response:" + coinMarketResponse);
                if (coinMarketResponse.data[currencyCode].Count == 0)
                {
                    return Results.NotFound();
                }
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

                    return Results.StatusCode((int)HttpStatusCode.InternalServerError);
                }
                app.Logger.LogInformation("Exchange API call returned response:" + exchangeResponse);
                //Add all other currencies and calculated rate to response collection
                foreach (var currency in exchangeResponse.rates)
                {
                    Currency currencyQuotes = new Currency
                    {
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
        }
    }
}
