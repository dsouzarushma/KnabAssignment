using CryptoAPI.Models;

namespace CryptoAPI
{
    public class ExchangesRateHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<ExchangesRateHttpClient> logger;
        public ExchangesRateHttpClient(HttpClient _httpClient, IConfiguration _configuration, ILogger<ExchangesRateHttpClient> logger)
        {
            httpClient = _httpClient;
            configuration = _configuration;
            this.logger = logger;
        }
        /// <summary>
        /// This method calls the exchange rate API to get the exchange rates for predefined set of currencies against base currency EURO 
        /// </summary>
        /// <param name="baseCurrency">This is the base currency defined in the application. It is set to EUR in configuration file.</param>
        /// <param name="exchangeCurrencies">It is set of predefined comma seperated currency values defined in the application i.e USD,BRL,GBP,AUD</param>
        /// <returns></returns>
        public async Task<ExchangeAPIResponse> GetExchangeRate(string baseCurrency, string exchangeCurrencies)
        {
            logger.LogInformation("ExchangeAPI call");
            var response = new  ExchangeAPIResponse();
            try
            {
               response  = await httpClient.GetFromJsonAsync<ExchangeAPIResponse>($"latest?access_key={configuration.GetValue<string>("ExchangeRateAPISettings:apiKeyValue")}&base={baseCurrency}&symbols={exchangeCurrencies}");
                logger.LogInformation("ExchangeAPI call completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError("ExchangeAPI call exception:"+ex.Message);
            }
            return response;
        }

    }
}
