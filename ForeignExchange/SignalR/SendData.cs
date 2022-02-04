using ExchangeModels;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForeignExchange.SignalR
{
    public class SendData
    {

        private readonly CurrencyTSRepository _currencyTSRepository;
        private readonly IHubContext<CurrencyHub> _hub;
        public SendData(CurrencyTSRepository currencyTSRepository, IHubContext<CurrencyHub> hub)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
            StartSendingData();

        }

        private void StartSendingData()
        {
            Task.Run(() => SendCandleData());
            Task.Run(() => SendEMAData());
            Task.Run(() => SendMinMaxData());
            Task.Run(() => SendCurrencyData());
        }

        private async Task SendCandleData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetCandleDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("BTCToCurrencyCandle", data);
                Thread.Sleep(2000);
            }
        }

        private async Task SendEMAData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetEMADataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("BTCToCurrencyEMA", data);
                Thread.Sleep(2000);
            }
        }

        private async Task SendMinMaxData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetMinMaxDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("CurrencyMinMax", data);
                Thread.Sleep(2000);
            }
        }

        private async Task SendCurrencyData()
        {
            while (true)
            {
                var data = await _currencyTSRepository.GetDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("Currency", data);
                Thread.Sleep(2000);
            }
        }
    }
}
