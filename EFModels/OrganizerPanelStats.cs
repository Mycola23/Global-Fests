namespace GlobalFests.EFModels
{
    public class OrganizerPanelStats
    {
        public GeneralRevenueStats GeneralStats { get; set; } = new GeneralRevenueStats();


        public List<MonthlySalesData> MonthlySales { get; set; } =  new List<MonthlySalesData>();

        public List<EventTypeSalesData> EventTypeSales { get; set; } = new List<EventTypeSalesData>();

        public List<EventCountrySalesData> EventCountrySales { get; set; } = new List<EventCountrySalesData>();

        public class MonthlySalesData
        {
            public string Month { get; set; }
            public int TicketsSold { get; set; }
            public decimal Revenue { get; set; }
        }

        public class GeneralRevenueStats
        {
            public int TotalTicketsSold { get; set; }
            public decimal TotalRevenue { get; set; }
            //public decimal AverageTicketPrice { get; set; }
            //public decimal PercentageTicketsSoldOverall { get; set; } 
        }

        public class EventTypeSalesData
        {
            public string EventType { get; set; }
            public int TicketsSold { get; set; }
            public decimal Revenue { get; set; }
        }

        public class EventCountrySalesData
        {
            public string CountryName { get; set; }

            public int TicketsSold { get; set; }

            public decimal Revenue { get; set; }
        }
    }
}
