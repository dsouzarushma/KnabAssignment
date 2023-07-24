namespace CryptoAPI.Models
{
    public class CoinAPIResponse
    {
        //public class Rootobject
        //{
            public Status status { get; set; }
            public Dictionary<string, List<CryptoCurrency>> data { get; set; }
        //   }

        public class Status
        {
            public DateTime timestamp { get; set; }
            public int error_code { get; set; }
            public object error_message { get; set; }
            public int elapsed { get; set; }
            public int credit_count { get; set; }
            public object notice { get; set; }
        }

        //public class Data
        //{
        //    public Dictionary<string,List<CryptoCurrency>> Currencies { get; set; }
        //}

        public class CryptoCurrency
        {
            //public int id { get; set; }
            //public string name { get; set; }
            //public string symbol { get; set; }
            //public string slug { get; set; }
            //public int num_market_pairs { get; set; }
            //public DateTime date_added { get; set; }
            //public List<Tag> tags { get; set; }
            //public int max_supply { get; set; }
            //public int circulating_supply { get; set; }
            //public int total_supply { get; set; }
            //public int is_active { get; set; }
            //public bool infinite_supply { get; set; }
            //public object platform { get; set; }
            //public int cmc_rank { get; set; }
            //public int is_fiat { get; set; }
            //public object self_reported_circulating_supply { get; set; }
            //public object self_reported_market_cap { get; set; }
            //public object tvl_ratio { get; set; }
            //public DateTime last_updated { get; set; }
            public Dictionary<string, Currency> quote { get; set; }
        }

        //public class Quote
        //{
        //    public Currency EUR { get; set; }
        //}

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

        //public class Tag
        //{
        //    public string slug { get; set; }
        //    public string name { get; set; }
        //    public string category { get; set; }
        //}

    }
}
