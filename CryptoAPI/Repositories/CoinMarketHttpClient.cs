using CryptoAPI.Models;

namespace CryptoAPI.Repositories
{
    public class CoinMarketHttpClient
    {
        private HttpClient httpClient;
        private IConfiguration configuration;
        private ILogger<CoinMarketHttpClient> logger;
        public CoinMarketHttpClient(HttpClient _httpClient, IConfiguration _configuration, ILogger<CoinMarketHttpClient> logger)
        {

            httpClient = _httpClient;
            configuration = _configuration;
            this.logger = logger;
        }
        /// <summary>
        /// This function calls the CoinMarket API to get the latest price in EUR for crypto currency code passed as input.
        /// </summary>
        /// <param name="currencyCode">This is user passed input value of Crypto Currency </param>
        /// <param name="baseCurrency">This is the base currency defined in the application. It is set to EUR in configuration file. </param>
        /// <returns></returns>
        public async Task<CoinAPIResponse> GetCryptoQuote(string currencyCode, string baseCurrency)
        {
            logger.LogInformation("Coin Market API call");
            var response = new CoinAPIResponse();
            try
            {
                response = await httpClient.GetFromJsonAsync<CoinAPIResponse>($"latest?symbol={currencyCode}&convert={baseCurrency}");
                logger.LogInformation("Coin Market API call completed successfully");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError("Coin Market API call exception:" + ex.Message);
                return new CoinAPIResponse
                {
                    status = new CoinAPIResponse.Status
                    {
                        error_code = (int)ex.StatusCode,
                        error_message = ex.Message
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError("Coin Market API call exception:" + ex.Message);
            }
            return response;
        }
    }
}
