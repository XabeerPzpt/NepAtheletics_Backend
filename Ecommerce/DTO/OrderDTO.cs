namespace Ecommerce.DTO
{
    // This model is used for passing only required parameters for recording order
    public class OrderDTO
    {
        public string Username { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; } 
        public int Quantity{ get; set; }
    }
    public class OrderDisplayDTO
    {
        public int ProductId { get; set; }  
        public string Product_name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total_price{ get; set; }
    }
} 
