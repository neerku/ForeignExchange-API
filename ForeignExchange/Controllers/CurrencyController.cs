using ForeignExchange.Logger;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR;
using ForeignExchange.SignalR.Hubs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ForeignExchange.Controllers
{
    [Route("currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private static bool IsSubscriptionRunning = false;
        private readonly CurrencyTSRepository _currencyTSRepository;
        private readonly IHubContext<CurrencyHub> _hub;
        private readonly Log _logger;
        private readonly TelemetryClient _telemetryClient;

        public CurrencyController(CurrencyTSRepository currencyTSRepository, IHubContext<CurrencyHub> hub)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
            _logger = new Log(_telemetryClient);
        }

        /// <summary>
        /// get candle data
        /// </summary>
        /// <param name="basecode"></param>
        /// <param name="convertedcode"></param>
        /// <returns>send currency data</returns>
        [HttpGet]
        [Route("candle/{basecode}/{convertedcode}")]
        public async Task<ActionResult> GetCandlestickData(string basecode, string convertedcode)
        {
            try
            {
                var currencyString = $"{basecode}-{convertedcode}";
                _logger.TrackTrace($"Get Candlestick data {currencyString}");
                var data = await _currencyTSRepository.GetDataAsync(currencyString);
                return Ok(data);
            }
            catch (OverflowException ex)
            {
                _logger.TrackException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"OverFlow Exception occurred {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.TrackException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Exception occurred :{ex.Message}" });
            }
        }

        /// <summary>
        /// Start subscription
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("start/subscription")]
        public ActionResult StartSendingCandlestickDataToclientsAsync()
        {
            try
            {
                if (!IsSubscriptionRunning)
                {
                    new SendData(_currencyTSRepository, _hub);
                    IsSubscriptionRunning = true;
                    _logger.TrackTrace("Subscription ran successfully");
                }
                return Ok();
            }
            catch (OverflowException ex)
            {
                _logger.TrackException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"OverFlow Exception occurred {ex.Message}" });
            }
            catch (Exception ex)
            {
                _logger.TrackException(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Exception occurred :{ex.Message}" });
            }
        }
    }
}