using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeModels;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForeignExchange.Controllers
{
    [Route("currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyTSRepository _currencyTSRepository;
        private readonly IHubContext<CurrencyHub> _hub;
        private static bool IsSubscriptionRunning = false;


        public CurrencyController(CurrencyTSRepository currencyTSRepository,IHubContext<CurrencyHub> hub)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
        }

        [HttpGet]
        [Route("candle/{basecode}/{convertedcode}")]
        public async Task<ActionResult> GetCandlestickData(string basecode, string convertedcode)
        {
            var currencyString = $"{basecode}-{convertedcode}";
            var data = await _currencyTSRepository.GetDataAsync(currencyString);
            return Ok(data);
        }

        [HttpGet]
        [Route("start/subscription")]
        public ActionResult StartSendingCandlestickDataToclientsAsync()
        {
            if (!IsSubscriptionRunning)
            {
                Task.Run(() => SendCandleData());
                Task.Run(() => SendEMAData());
                IsSubscriptionRunning = true;
            }

            return Ok();
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
    }
}
