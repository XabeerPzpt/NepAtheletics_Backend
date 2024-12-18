using Ecommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/AdminOrderController")]
    [Authorize]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly ICompanyRepository _repository;

        public AdminOrderController(ICompanyRepository repository)
        {
            _repository = repository;
        }

        // Get Item ids addded to cart :
        [HttpGet("confirmedOrders")]
        public async Task<IActionResult> GetConfirmedOrders()
        {
            var result = await _repository.GetConfirmedOrders();
            return Ok(result);
        }

        [HttpPut("makeDelivery")]
        public async Task<IActionResult> DeliverOrder(int orderId)
        {
            var result = await _repository.MakeDelivery(orderId);
            return Ok(result);
        }
    }
}
