using System.Text.Json;

namespace Ecommerce.DTO
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; } 
        public string Message { get; set; }
        public override string ToString()       //inbuilt method in C#
        {
            return JsonSerializer.Serialize(this); //passing the current ErrorDetails object to this method
        }
    }
}
