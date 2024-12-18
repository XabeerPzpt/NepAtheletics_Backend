using Ecommerce.DTO;
namespace Ecommerce.Entities
{
    public class Users : RegisterDTO
    {
        public string Role { get; set; }
        public bool Is_Block {  get; set; } 
        public bool Is_Active { get; set; } 
        public string Created_By { get; set; }
        public DateTime? Created_Time { get; set; }
        public string Updated_By { get; set; } 
        public DateTime? Updated_Time { get; set; }


    }
}
