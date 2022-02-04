using ExchangeModels;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR;
using ForeignExchange.SignalR.Hubs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForeignExchange.Controllers
{
    [Route("currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private static bool IsSubscriptionRunning = false;
        private readonly CurrencyTSRepository _currencyTSRepository;
        private readonly IHubContext<CurrencyHub> _hub;
        private readonly TelemetryClient _telemetryClient; 

            public CurrencyController(CurrencyTSRepository currencyTSRepository, IHubContext<CurrencyHub> hub, TelemetryClient telemetryClient)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
            _telemetryClient = telemetryClient;
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
                new SendData(_currencyTSRepository, _hub);
                IsSubscriptionRunning = true;
            }

            return Ok();
        }


    }
}