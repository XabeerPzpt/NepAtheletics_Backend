using Ecommerce.DTO;
using Ecommerce.Filters;
using Ecommerce.Repository;
using Ecommerce.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/UserOrders")]
    [Authorize]
    [ApiController]

    public class UserOrderController : ControllerBase
    {
        public readonly ICompanyRepository _repository;
        public UserOrderController(ICompanyRepository repository) => _repository = repository;

        // Inserting into orders table:
        [HttpPost("insertOrderRecord")]
        //  [ServiceFilter(typeof(OrderValidationFilter))]     // Applying the custom validation filter here
        public async Task<IActionResult> InsertOrders(OrderDTO orderData)
        {
            var result = await _repository.InsertOrders(orderData);
            return Ok(result);
        }

        //Adding Product Ids into cart of DB
        [HttpPost("addItemsToCart")] 
        public async Task<IActionResult> AddToCart([FromBody] CartDetails cartDetails)
        {
            var result = await _repository.AddToCart(cartDetails);
            return Ok(result);
        }

        // Get Item ids addded to cart :
        [HttpGet("itemsAddedToCart/{username}")]
        public async Task<IActionResult> GetIdsAddedToCart(string username)
        {
         var result = await _repository.GetItemsAddedToCart(username);
            return Ok(result);
        }

        // Get Pending Orders of a User:
        [HttpGet("getOrderByUsername/{username}")]
        public async Task<IActionResult> GetOrderByUsername([FromRoute] string username)
        {
            var result = await _repository.GetOrderByUsername(username);
            return Ok(result);
        }

        // Get all Orders of a User by username:
        [HttpGet("getAllOrdersByUsername/{username}")]
        public async Task<IActionResult> GetAllOrdersByUsername([FromRoute] string username)
        {
            var result = await _repository.GetAllOrdersByUsername(username);
            return Ok(result);
        }

        //Increase Quantity by +1 of product:
        [HttpPut("increaseQty")]
        public async Task<IActionResult> IncreaseQty([FromBody] ChangeQtyDTO obj)
        {
            var result = await _repository.IncreaseQty(obj);
            return Ok(result);
        }

        //Increase Quantity by +1 of product:
        [HttpPut("decreaseQty")]
        public async Task<IActionResult> DecreaseQty([FromBody] ChangeQtyDTO obj)
        {
            var result = await _repository.DecreaseQty(obj);
            return Ok(result);
        }

        //Delete a product from My Cart 
        [HttpDelete("deleteFromCart")]
        public async Task<IActionResult> DeleteFromCart([FromBody] ChangeQtyDTO obj)
        {
            var result = await _repository.DeleteFromCart(obj);
            return Ok(result);
        }

        // Get Total Orders Per Month 
        [HttpGet("getOrdersPerMonth/{year}")]
        public async Task<IActionResult> GetOrdersPerMonth(int year)
        {
            var total_users = await _repository.GetTotalOrdersPerMonth(year);
            return Ok(total_users);
        }


        //Confriming order from User:
        [HttpPut("confirmOrder/{username}")]
        public async Task<IActionResult> ConfirmOrder(string username)
        {
            var result = await _repository.ConfirmOrder(username);
            return Ok(result);
        }
    }
}
