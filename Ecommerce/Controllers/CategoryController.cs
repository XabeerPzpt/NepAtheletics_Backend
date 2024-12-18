using Ecommerce.Entities;
using Ecommerce.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecommerce.Controllers
{
    [Route("api/Categories")]
    [Authorize]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICompanyRepository _repository;  
        public CategoryController(ICompanyRepository repository) => _repository = repository;

        // Get category of products:
        [HttpGet("get_Category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory()
        {
            var category = await _repository.GetCategory();
            return Ok(category);
        }


        // Get sub category:
        [HttpGet("get_subCategory/{CategoryId}")]
        public async Task<IActionResult> GetSubCategory(int CategoryId)
        {
            var SubCategory = await _repository.GetSubCategory(CategoryId);
            return Ok(SubCategory);
        }

        // Get product :
        [HttpGet("get_product/{SubCategoryId}")]
        public async Task<IActionResult> GetProduct(int SubCategoryId)
        {
            var product = await _repository.GetProduct(SubCategoryId);
            return Ok(product);
        }

        //Get all product by Sport Id:
        [HttpGet("get_products/{SportId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsBySportId(int SportId)
        {
            var products = await _repository.GetProductsBySportId(SportId);
            return Ok(products);
        }

        //Get all product details:
        [HttpGet("get_all_products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllProducts();
            return Ok(products);
        }

        //Get all product by Sport Id:
        [HttpGet("get_image_url/{ProductId}")]
        public async Task<IActionResult> GetProductImageUrl(int ProductId)
        {
            var imgUrl = await _repository.GetProductImageUrl(ProductId);
            return Ok(imgUrl);
        }


        //Get Product details by it's id :
        [HttpGet("get_product_by_id/{ProductId}")]
        public async Task<IActionResult> GetProductByProductId(int ProductId)
        {
            var product = await _repository.GetProductByProductId(ProductId);
            return Ok(product);
        }

        //Adding new category:
        [HttpPost("add_category")]
        public async Task<IActionResult> AddCategory(string name)
        {
            var result = await _repository.AddCategory(name);
            if (result.Code == "000") return Ok(result);
            else return BadRequest(result);
        }

        //Adding new subcategory:
        [HttpPost("add_subCategory")]
        public async Task<IActionResult> AddSubCategory([FromBody] equipment_and_clothing data)
        {
            try
            {
                var result = await _repository.AddSubCategory(data);
                if (String.Compare(result.Code, "000",StringComparison.OrdinalIgnoreCase)==0)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e) {
                return Ok(new
                {
                    Code = HttpStatusCode.InternalServerError,
                    Message= e.Message
                });
            }
        }


        //Adding new product:
        [HttpPost("add_product")]
        public async Task<IActionResult> AddProduct([FromBody] Product data)
        {
            try
            {
                var result = await _repository.AddProduct(data);
                if (result.Code == "000")
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.InternalServerError,
                    Message = e.Message
                });
            }
        }



        //Delete Category:
        [HttpDelete("delete_category/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int id)
        {
            var result = await _repository.DeleteCategory(id);
            if (result.Code=="000")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        //Delete Sub Category:
        [HttpDelete("delete_subCategory/{id}")]
        public async Task<IActionResult> DeleteSubCategory([FromRoute] int id)
        {
            try
            {
                var result = await _repository.DeleteSubCategory(id);
                if (result.Code == "000")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.InternalServerError,
                    Message = e.Message
                });
            }
        }

        //Delete Product:
        [HttpDelete("delete_product/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var result = await _repository.DeleteProduct(id);
                if (result.Code == "000")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.InternalServerError,
                    Message = e.Message
                });
            }
        }


        //Update Category
        [HttpPut("update_category")]
        public async Task<IActionResult> UpdateCategory(Sports sportObject)
        {
            try
            {
                var result = await _repository.UpdateCategory(sportObject);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.InternalServerError,
                    ExceptionMessage = e.Message
                });
            }
        }


        //Update Sub Category
        [HttpPut("update_subCategory")]
        public async Task<IActionResult> UpdateSubCategory(equipment_and_clothing data)
        {
            try
            {
                var result = await _repository.UpdateSubCategory(data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.ExpectationFailed,
                    ExceptionMessage = e.Message
                });
            }
        }


        //Update Product
        [HttpPut("update_Product")]
        public async Task<IActionResult> UpdateProduct(Product data)
        {
            try
            {
                var result = await _repository.UpdateProduct(data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    Code = HttpStatusCode.ExpectationFailed,
                    ExceptionMessage = e.Message
                });
            }
        }


        //This API helps to get a row for particular table nased on its id .
        //It is used to get name while clicking edit button
        // Get Item name 
        [HttpGet("get_item/{table}/{id}")]
        public async Task<IActionResult> GetItem(int id, string table)
        {
            try
            {
                switch (table.ToLower())
                {
                    case "sports":
                        var sports = await _repository.GetItem<Sports>(id, table);
                        return Ok(sports);
                    case "equipment_and_clothing":
                        var equipment = await _repository.GetItem<equipment_and_clothing>(id, table);
                        return Ok(equipment);
                    case "product":
                        var product = await _repository.GetItem<Product>(id, table);
                        return Ok(product);
                    default:
                        return BadRequest("Invalid Category!");
                }
            }catch(Exception e)
            {
                return Ok(e.Message);
            }
        }
    }
}
