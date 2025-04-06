namespace DietDoHongTran.Models
{
    public class ProductStatisticsViewModel
    {
        public DateTime Date { get; set; }
        public int TotalProducts { get; set; }
        public int TotalInStock { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalValueInStock { get; set; }

        public List<Product> TopSellingProducts { get; set; }
        public List<Product> OutOfStockProducts { get; set; }
        public List<Product> LowStockProducts { get; set; }

        public List<CategoryStatistics> ProductsByCategory { get; set; }
    }

    public class CategoryStatistics
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSold { get; set; }
        public int TotalInStock { get; set; }
        public decimal TotalValueInStock { get; set; }
    }
}
