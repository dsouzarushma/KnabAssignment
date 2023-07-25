# CryptoAPI

This is a minimal API application to return quote for user submitted crypto currency in different currencies.
Before running the api set the path for log file in appSettings.json file(LogPath).
This API is protected using API key authentication.
API key to be used is: Name-'X-Api-Key' Value-'54E4E03F8F2448619EDB8EF225E866F1'
It expects the input parameter crypto currency code.
Coin Market API and ExchangeRate APIs are used internally to get the final desired result.
API keys for both the api are kept in appSettings.json file you can use your own key.
Base Currency to get quote from coin market is used as EUR as exhange API was not accepting base currency USD for my account.
