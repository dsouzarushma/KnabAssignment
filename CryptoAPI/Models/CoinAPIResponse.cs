namespace CryptoAPI.Models
{
    public class CoinAPIResponse
    {
            public Status status { get; set; }
            public Dictionary<string, List<CryptoCurrency>> data { get; set; }

        public class Status
        {
            public DateTime timestamp { get; set; }
            public int error_code { get; set; }
            public object error_message { get; set; }
            public int elapsed { get; set; }
            public int credit_count { get; set; }
            public object notice { get; set; }
        }

        public class CryptoCurrency
        {
            public Dictionary<string, Currency> quote { get; set; }
        }


        public class Currency
        {
            public float price { get; set; }
            public float volume_24h { get; set; }
            public float volume_change_24h { get; set; }
            public float percent_change_1h { get; set; }
            public float percent_change_24h { get; set; }
            public float percent_change_7d { get; set; }
            public float percent_change_30d { get; set; }
            public float percent_change_60d { get; set; }
            public float percent_change_90d { get; set; }
            public float market_cap { get; set; }
            public float market_cap_dominance { get; set; }
            public float fully_diluted_market_cap { get; set; }
            public object tvl { get; set; }
            public DateTime last_updated { get; set; }
        }

    }
}
