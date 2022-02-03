namespace ExchangeModels
{
    public class Constants
    {
        #region Databases

        public const string CurrencyDatabase = "exchange";

        #endregion Databases

        #region Collections

        public const string CurrencyCollection = "currency";
        public const string CurrencyTSCollection = "currency";

        #endregion Collections

        public static readonly string[] CurrencySymbol = new[]
      {
            "BTC-USD","BTC-GBP","BTC-INR","BTC-KYD","BTC-AUD","BTC-CNY","BTC-NZD","BTC-EUR","BTC-SGD","BTC-BRL"
        };
    }
}