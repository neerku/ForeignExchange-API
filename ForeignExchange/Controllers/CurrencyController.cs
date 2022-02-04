using ExchangeModels;
using ForeignExchange.Logger;
using ForeignExchange.Repositories;
using ForeignExchange.SignalR;
using ForeignExchange.SignalR.Hubs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
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

        public CurrencyController(CurrencyTSRepository currencyTSRepository, IHubContext<CurrencyHub> hub,TelemetryClient telemetryClient)
        {
            _currencyTSRepository = currencyTSRepository;
            _hub = hub;
            _logger = new Log(telemetryClient);
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
                var abc = await _currencyTSRepository.GetEMADataAsync(Constants.CurrencySymbol);
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
                    StartSendingData();
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