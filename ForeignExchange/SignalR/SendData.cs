using ExchangeModels;
using ForeignExchange.Logger;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR.Hubs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace ForeignExchange.SignalR
{
    public class SendData
    {
        private readonly CurrencyTSRepository _currencyTSRepository;
        private readonly IHubContext<CurrencyHub> _hub;
        private readonly Log _logger;
        private readonly TelemetryClient _telemetryClient;

        public SendData(CurrencyTSRepository currencyTSRepository, IHubContext<CurrencyHub> hub)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
            StartSendingData();
        }

        /// <summary>
        /// send candle data
        /// </summary>
        /// <returns></returns>
        private async Task SendCandleData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetCandleDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("BTCToCurrencyCandle", data);
                _logger.TrackTrace("SendCandleData sent successfully");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// send currency data
        /// </summary>
        /// <returns></returns>
        private async Task SendCurrencyData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("Currency", data);
                _logger.TrackTrace("SendCurrencyData sent successfully");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// send ema data
        /// </summary>
        /// <returns></returns>
        private async Task SendEMAData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetEMADataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("BTCToCurrencyEMA", data);
                _logger.TrackTrace("SendEMAData sent successfully");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// send min max data
        /// </summary>
        /// <returns></returns>
        private async Task SendMinMaxData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetMinMaxDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("CurrencyMinMax", data);
                _logger.TrackTrace("SendMinMaxData sent successfully");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// sending data
        /// </summary>
        private void StartSendingData()
        {
            Task.Run(() => SendCandleData());
            Task.Run(() => SendEMAData());
            Task.Run(() => SendMinMaxData());
            Task.Run(() => SendCurrencyData());
        }
    }
}