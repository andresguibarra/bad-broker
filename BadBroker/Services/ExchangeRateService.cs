using BadBroker.Models;

namespace BadBroker.Services
{
    public class ExchangeRateCalculatorService
    {
        public RatesReturnModel GetBestToolAndRevenue(Dictionary<DateTime, Dictionary<string, decimal>> ratesByDatetime, decimal initialMoney)
        {
            var (buyDate, sellDate, tool, revenue) = GetBestDatesForRevenue(ratesByDatetime, initialMoney);

            var ratesReturnModel = new RatesReturnModel
            {
                BuyDate = buyDate,
                SellDate = sellDate,
                Tool = tool,
                Revenue = revenue,
                Rates = ratesByDatetime.Select(r => new Rate
                {
                    Date = r.Key,
                    Rub = r.Value["RUB"],
                    Eur = r.Value["EUR"],
                    Gbp = r.Value["GBP"],
                    Jpy = r.Value["JPY"]
                }).ToList()
            };

            return ratesReturnModel;
        }

        private (DateTime buyDate, DateTime sellDate, string tool, decimal revenue) GetBestDatesForRevenue(Dictionary<DateTime, Dictionary<string, decimal>> ratesByDatetime, decimal initialMoney)
        {
            DateTime bestBuyDate = DateTime.MinValue;
            DateTime bestSellDate = DateTime.MinValue;
            string bestTool = "";
            decimal maxRevenue = 0;
            var ratesList = ratesByDatetime.ToList();

            // Iterate through all possible buy dates
            for (int buyIndex = 0; buyIndex < ratesList.Count - 1; buyIndex++)
            {
                var buyDate = ratesList[buyIndex].Key;
                var buyRates = ratesList[buyIndex].Value;

                // Iterate through all currency tools for the given buy date
                foreach (var buyRate in buyRates)
                {
                    var tool = buyRate.Key;
                    var rate = buyRate.Value;
                    var currencyAmount = initialMoney * rate;
                    decimal brokerFee = 0;

                    // Iterate through all possible sell dates for the given currency tool and buy date
                    for (int sellIndex = buyIndex + 1; sellIndex < ratesList.Count; sellIndex++)
                    {
                        var sellDate = ratesList[sellIndex].Key;
                        var sellRates = ratesList[sellIndex].Value;

                        if (sellRates.TryGetValue(tool, out var sellRate))
                        {
                            brokerFee = (sellDate - buyDate).Days;
                            var revenue = CalculateRevenue(currencyAmount, sellRate, brokerFee, initialMoney);

                            if (revenue > maxRevenue)
                            {
                                maxRevenue = revenue;
                                bestBuyDate = buyDate;
                                bestSellDate = sellDate;
                                bestTool = tool;
                            }
                        }
                    }
                }
            }

            return (bestBuyDate, bestSellDate, bestTool, maxRevenue);
        }

        // Calculates the revenue by converting the currencyAmount back to the original currency, subtracting the broker fee and initialMoney
        private decimal CalculateRevenue(decimal currencyAmount, decimal sellRate, decimal brokerFee, decimal initialMoney)
        {
            if (sellRate == 0)
            {
                return 0;
            }
            return (currencyAmount / sellRate) - brokerFee - initialMoney;
        }

    }
}
