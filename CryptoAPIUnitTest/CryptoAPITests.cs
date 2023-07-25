using CryptoAPI;
using CryptoAPI.Models;
using CryptoAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http.Json;
using static CryptoAPI.Models.CoinAPIResponse;

namespace CryptoAPIUnitTest
{
    public class CryptoAPITests
    {
        [Fact]
        public async Task GetQuote_FailAuthorization()
        {
            // Arrange
           
            var currencyCode = "BTC";
            await using var application = new WebApplicationFactory<Program>();
            
            using var client = application.CreateClient();

            var response = await client.GetAsync("/getQuote/"+currencyCode);

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task GetQuote_PassAuthorization()
        {
            // Arrange
           
            var currencyCode = "BTC";
            var authenticationKey = "54E4E03F8F2448619EDB8EF225E866F1";
            await using var application = new WebApplicationFactory<Program>();
            
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key",authenticationKey);
            var response = await client.GetAsync("/getQuote/" + currencyCode);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetQuote_InvalidCurrencyCode()
        {
            // Arrange

            var currencyCode = "BTC45";
            var authenticationKey = "54E4E03F8F2448619EDB8EF225E866F1";
            await using var application = new WebApplicationFactory<Program>();

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", authenticationKey);
            var response = await client.GetAsync("/getQuote/" + currencyCode);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Fact]
        public async Task GetQuote_CurrencyCodeNotFound()
        {
            // Arrange

            var currencyCode = "AB1";
            var authenticationKey = "54E4E03F8F2448619EDB8EF225E866F1";
            await using var application = new WebApplicationFactory<Program>();

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", authenticationKey);
            var response = await client.GetAsync("/getQuote/" + currencyCode);

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task GetQuote_ValidCurrencyCode()
        {
            // Arrange

            var currencyCode = "BTC";
            var authenticationKey = "54E4E03F8F2448619EDB8EF225E866F1";
            await using var application = new WebApplicationFactory<Program>();

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", authenticationKey);
            var response = await client.GetAsync("/getQuote/" + currencyCode);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetQuote_ExternalAPIFailure()
        {
            // Arrange

            var currencyCode = "BTC";
            var authenticationKey = "54E4E03F8F2448619EDB8EF225E866F1";
            await using var application = new WebApplicationFactory<Program>();

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", authenticationKey);
            var response = await client.GetAsync("/getQuote/" + currencyCode);
            Assert.NotEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}