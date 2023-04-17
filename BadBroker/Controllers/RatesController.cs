using BadBroker.Models;
using BadBroker.Services;
using BadBroker.Strategies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ValidationResult = BadBroker.Strategies.ValidationResult;

namespace BadBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly ExchangeRateDataLoaderService _exchangeRateDataLoaderService;
        private readonly ExchangeRateCalculatorService _exchangeRateCalculatorService;
        public RatesController(ExchangeRateDataLoaderService exchangeRateDataLoaderService, ExchangeRateCalculatorService exchangeRateCalculatorService)
        {
            _exchangeRateCalculatorService = exchangeRateCalculatorService;
            _exchangeRateDataLoaderService = exchangeRateDataLoaderService;
        }

        [HttpGet("best")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RatesReturnModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> Get([FromQuery, Required] DateTime? startDate, [FromQuery, Required] DateTime? endDate, [FromQuery, Required] decimal? moneyUsd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validationStrategies = new List<IValidationStrategy>
                {
                    new StartDateBeforeEndDateValidation(),
                    new DateDifferenceValidation(60)
                };

            foreach (var strategy in validationStrategies)
            {
                var validationResult = strategy.Validate((DateTime)startDate, (DateTime)endDate);

                if (validationResult != ValidationResult.Success)
                {
                    return BadRequest(validationResult.ErrorMessage);
                }
            }

            var exchangeRates = await _exchangeRateDataLoaderService.GetExchangeRatesForMultipleDates((DateTime)startDate, (DateTime)endDate);
            var result = _exchangeRateCalculatorService.GetBestToolAndRevenue(exchangeRates, (decimal)moneyUsd);

            return Ok(result);
        }
    }
}
