namespace DietDoHongTran.Models
{
    public class ProductStatistic
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }  // Ngày thống kê
        public int TotalProducts { get; set; }
        public int TotalInStock { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalValueInStock { get; set; }
    }
}
