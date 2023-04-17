using BadBroker.Services;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace BadBroker.Tests
{
    public class ExchangeRateDataLoaderServiceTests
    {
        private readonly Mock<IRestClient> _restClientMock;
        private readonly ExchangeRateDataLoaderService _exchangeRateDataLoaderService;

        public ExchangeRateDataLoaderServiceTests()
        {
            _restClientMock = new Mock<IRestClient>();
            _exchangeRateDataLoaderService = new ExchangeRateDataLoaderService(_restClientMock.Object);
        }

        [Fact]
        public async Task GetExchangeRatesForMultipleDates_ValidDates_ReturnsExpectedExchangeRatesAsync()
        {
            var startDate = new DateTime(2022, 1, 1);
            var endDate = new DateTime(2022, 1, 3);
            var sampleResponse = "{\"rates\": {\"RUB\": 70.0, \"EUR\": 0.85, \"GBP\": 0.75, \"JPY\": 110.0}}";
            var expectedRate = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(JObject.Parse(sampleResponse)["rates"].ToString());

            _restClientMock
                .Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), default))
                .ReturnsAsync(new RestResponse { Content = sampleResponse, StatusCode = HttpStatusCode.OK, ResponseStatus = ResponseStatus.Completed, IsSuccessStatusCode = true });

            var result = await _exchangeRateDataLoaderService.GetExchangeRatesForMultipleDates(startDate, endDate);

            Assert.Equal(3, result.Count);
            foreach (var rate in result.Values)
            {
                Assert.Equal(expectedRate, rate);
            }
        }
    }
}
