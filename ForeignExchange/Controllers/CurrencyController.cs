using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForeignExchange.Models;
using ForeignExchange.Models.Projections;
using ForeignExchange.Repositories;
using Microsoft.AspNetCore.Mvc;
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

        public CurrencyController(CurrencyRepository currencyRepository, CurrencyTSRepository currencyTSRepository)
        {
            _currencyRepository = currencyRepository;
            _currencyTSRepository = currencyTSRepository;
        }

        [HttpGet]
        [Route("candle/{basecode}/{convertedcode}")]
        public async Task<ActionResult> GetCandlestickData(string basecode, string convertedcode)
        {
            var currencyString = $"{basecode}-{convertedcode}";
            var data = await _currencyTSRepository.GetDataAsync(currencyString);

            return Ok(data);
        }
    }
}
