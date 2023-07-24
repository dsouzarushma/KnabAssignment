namespace CryptoAPI.Models
{
    public class ExchangeAPIResponse
    {
            public bool success { get; set; }
            public int timestamp { get; set; }
            public string _base { get; set; }
            public string date { get; set; }
            public Dictionary<string, float> rates { get; set; }

    }
}
