using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BadBroker.Services
{
    public class ExchangeRateDataLoaderService
    {
        private readonly string AppId = "13fa4bb6e96143dea908a4605e865a88";
        private readonly List<string> Currencies = new List<string>() { "RUB", "EUR", "GBP", "JPY" };
        private readonly string ApiUrl = "https://openexchangerates.org/api/";
        private readonly IRestClient _restClient;
        private readonly string _cachePath;

        public ExchangeRateDataLoaderService(IRestClient restClient)
        {
            _restClient = restClient;
            _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
            if (!Directory.Exists(_cachePath))
            {
                Directory.CreateDirectory(_cachePath);
            }
        }

        public async Task<Dictionary<DateTime, Dictionary<string, decimal>>> GetExchangeRatesForMultipleDates(DateTime startDate, DateTime endDate)
        {
            var tasks = new List<Task<KeyValuePair<DateTime, Dictionary<string, decimal>>>>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var localDate = currentDate;

                tasks.Add(Task.Run(async () =>
                {
                    var externalRates = await GetExchangeRate(localDate);

                    return new KeyValuePair<DateTime, Dictionary<string, decimal>>(localDate, externalRates);
                }));

                currentDate = currentDate.AddDays(1);
            }

            await Task.WhenAll(tasks);

            var results = new Dictionary<DateTime, Dictionary<string, decimal>>();

            foreach (var task in tasks)
            {
                var result = task.Result;
                results[result.Key] = result.Value;
            }

            return results;
        }

        private async Task<Dictionary<string, decimal>> GetExchangeRate(DateTime date)
        {
            var fileName = $"{date:yyyy-MM-dd}.json";
            var filePath = Path.Combine(_cachePath, fileName);

            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, decimal>>(content);
            }
            else
            {
                var exchangeRate = await FetchExchangeRate(date);
                var json = JsonConvert.SerializeObject(exchangeRate);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        await streamWriter.WriteAsync(json);
                    }
                }
                return exchangeRate;
            }
        }

        private async Task<Dictionary<string, decimal>> FetchExchangeRate(DateTime date)
        {
            var requestUrl = $"{ApiUrl}historical/{date.ToString("yyyy-MM-dd")}.json?&app_id={AppId}&symbols={string.Join(",", Currencies)}";
            var request = new RestRequest(requestUrl, Method.Get);
            request.AddHeader("accept", "application/json");
            var response = await _restClient.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var ratesObject = JObject.Parse(response.Content)["rates"];
                var result = ratesObject?.ToObject<Dictionary<string, decimal>>();
                return result;
            }
            else
            {
                throw new Exception("Error when obtaining the external exchange rates");
            }
        }
    }
}
