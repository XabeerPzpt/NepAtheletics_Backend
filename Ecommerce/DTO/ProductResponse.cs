using System.Net;

namespace Ecommerce.DTO
{
    public class ProductResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public object Item {  get; set; }
    }
}
