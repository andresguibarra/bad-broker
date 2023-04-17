using BadBroker.Controllers;
using BadBroker.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestSharp;

namespace BadBroker.Tests
{
    public class RatesControllerValidationTests
    {
        private readonly RatesController _controller;

        public RatesControllerValidationTests()
        {
            var restClientMock = new Mock<IRestClient>();
            var exchangeRateDataLoaderService = new ExchangeRateDataLoaderService(restClientMock.Object);
            var exchangeRateCalculatorService = new ExchangeRateCalculatorService();
            _controller = new RatesController(exchangeRateDataLoaderService, exchangeRateCalculatorService);
        }

        [Fact]
        public async Task Get_StartDateAfterEndDate_ReturnsBadRequestResultAsync()
        {
            DateTime startDate = new DateTime(2020, 1, 10);
            DateTime endDate = new DateTime(2020, 1, 1);
            decimal moneyUsd = 100m;

            var result = await _controller.Get(startDate, endDate, moneyUsd);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_DateDifferenceTooLarge_ReturnsBadRequestResultAsync()
        {
            DateTime startDate = new DateTime(2020, 1, 1);
            DateTime endDate = startDate.AddDays(61);
            decimal moneyUsd = 100m;

            var result = await _controller.Get(startDate, endDate, moneyUsd);

            Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
