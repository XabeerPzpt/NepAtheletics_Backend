namespace Ecommerce.DTO
{
    public class OrderHistoryDTO
    {
        public DateTime OrderDate { get; set; }
        public string product { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal totalPrice { get; set; }
        public string status { get; set; }
    }
}
 