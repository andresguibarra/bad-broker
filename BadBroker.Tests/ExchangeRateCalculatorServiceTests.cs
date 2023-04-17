using BadBroker.Services;

namespace BadBroker.Tests
{
    public class ExchangeRateCalculatorServiceTests
    {
        private readonly ExchangeRateCalculatorService _calculatorService;

        public ExchangeRateCalculatorServiceTests()
        {
            _calculatorService = new ExchangeRateCalculatorService();
        }

        [Fact]
        public void CalculateBestToolAndRevenue_WithRUB_ReturnsExpectedRevenue()
        {
            var ratesByDatetime = new Dictionary<DateTime, Dictionary<string, decimal>>
            {
                { new DateTime(2014, 12, 15), new Dictionary<string, decimal> { { "RUB", 60.17m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 16), new Dictionary<string, decimal> { { "RUB", 72.99m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 17), new Dictionary<string, decimal> { { "RUB", 66.01m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 18), new Dictionary<string, decimal> { { "RUB", 61.44m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 19), new Dictionary<string, decimal> { { "RUB", 59.79m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 20), new Dictionary<string, decimal> { { "RUB", 59.79m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 21), new Dictionary<string, decimal> { { "RUB", 59.79m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 22), new Dictionary<string, decimal> { { "RUB", 54.78m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
                { new DateTime(2014, 12, 23), new Dictionary<string, decimal> { { "RUB", 54.80m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 0m } } },
            };
            decimal initialMoney = 100;

            var result = _calculatorService.GetBestToolAndRevenue(ratesByDatetime, initialMoney);

            Assert.True(new DateTime(2014, 12, 16) == result.BuyDate, $"Expected BuyDate to be 2014-12-16, but found {result.BuyDate} in the RUB/USD scenario.");
            Assert.True(new DateTime(2014, 12, 22) == result.SellDate, $"Expected SellDate to be 2014-12-22, but found {result.SellDate} in the RUB/USD scenario.");
            Assert.True("RUB" == result.Tool, $"Expected currency tool to be RUB, but found {result.Tool} in the RUB/USD scenario.");
            Assert.True(27.24205914567360350492880613m == result.Revenue, $"Expected Revenue to be 27.24205914567360350492880613, but found {result.Revenue} in the RUB/USD scenario.");
        }

        [Fact]
        public void CalculateBestToolAndRevenue_WithJPY_ReturnsExpectedRevenue()
        {
            var ratesByDatetime = new Dictionary<DateTime, Dictionary<string, decimal>>
            {
                { new DateTime(2012, 1, 5), new Dictionary<string, decimal> { { "RUB", 0m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 40.00m } } },
                { new DateTime(2012, 1, 7), new Dictionary<string, decimal> { { "RUB", 0m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 35.00m } } },
                { new DateTime(2012, 1, 19), new Dictionary<string, decimal> { { "RUB", 0m }, { "EUR", 0m }, { "GBP", 0m }, { "JPY", 30.00m } } },
            };
            decimal initialMoney = 50;

            var result = _calculatorService.GetBestToolAndRevenue(ratesByDatetime, initialMoney);

            Assert.True(new DateTime(2012, 1, 5) == result.BuyDate, $"Expected BuyDate to be 2012-01-05, but found {result.BuyDate} in the JPY/USD scenario.");
            Assert.True(new DateTime(2012, 1, 7) == result.SellDate, $"Expected SellDate to be 2012-01-07, but found {result.SellDate} in the JPY/USD scenario.");
            Assert.True("JPY" == result.Tool, $"Expected currency tool to be JPY, but found {result.Tool} in the JPY/USD scenario.");
            Assert.True(5.142857142857142857142857143m == result.Revenue, $"Expected Revenue to be 5.142857142857142857142857143, but found {result.Revenue} in the JPY/USD scenario.");
        }
    }
}