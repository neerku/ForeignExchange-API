using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeModels;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForeignExchange.Controllers
{
    [Route("currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyRepository _currencyRepository;
        private readonly CurrencyTSRepository _currencyTSRepository;
        private IHubContext<CurrencyCandlestickHub> _hub;


        public CurrencyController(CurrencyRepository currencyRepository, CurrencyTSRepository currencyTSRepository,IHubContext<CurrencyCandlestickHub> hub)
        {
            _currencyRepository = currencyRepository;
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
        [Route("candle/subscription")]
        public ActionResult StartSendingCandlestickDataToclientsAsync()
        {
            Task.Run(() => SendData());
            return Ok();
        }

        private async Task SendData()
        {
            while (true)
            {
                var data2 = await _currencyTSRepository.GetDataAsync(Constants.CurrencySymbol);
                await _hub.Clients.All.SendAsync("BTCToCurrencyCandle", data2[0]);
            }
        }
    }
}
