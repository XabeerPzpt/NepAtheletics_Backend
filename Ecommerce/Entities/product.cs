namespace Ecommerce.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Equipment_id { get; set; }
        public float Price {  get; set; }
        public string Description { get; set; }  
        public string Image_url { get; set; }
    }
}
