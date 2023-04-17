namespace BadBroker.Models
{
    public class RatesReturnModel
    {
        public List<Rate> Rates { get; set; }
        public DateTime BuyDate { get; set; }   
        public DateTime SellDate { get; set; }
        public string Tool { get; set; }
        public decimal Revenue { get; set; }

    }
}
