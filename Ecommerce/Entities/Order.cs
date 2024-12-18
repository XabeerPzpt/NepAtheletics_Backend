using Ecommerce.DTO;

// This model is used for displaying order records in admin site 
namespace Ecommerce.Entities
{
    public class Order : OrderDTO
    {
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public bool Status { get; set; }
    } 
}
