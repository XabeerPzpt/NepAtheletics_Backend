using Dapper;
using Ecommerce.DatabaseFunctions;
using Ecommerce.DTO;
using Ecommerce.Entities;
using System.Data;
using System.Security.Claims;
using System.Net;
using Ecommerce.Services;

namespace Ecommerce.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;                    //For accessing DB connection method from DapperContext
        private readonly IHttpContextAccessor _contextAccessor;     //For Accessing claims from the current HTTP context
        private readonly IAuthenticationService _authService;       //For Accessing generate token method from AuthenticationService class

        //Constructor:
        public CompanyRepository(DapperContext context, IConfiguration configuration, IHttpContextAccessor contextAccessor, IAuthenticationService authService)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _authService = authService;
        }


        // Get all the users 
        public async Task<IQueryable<RegisterDTO>> GetUsers()
        {
            var query = "select * from Users";      
            var users = await RunSqlQuery<RegisterDTO>(query);
            return users;

        }

        // Get all the users for Admin
        public async Task<IQueryable<Users>> GetUsersForAdmin()
        {
            var query = "select * from Users";
            var users = await RunSqlQuery<Users>(query); 
            return users;

        } 


        // Get total users per month
        public async Task<IQueryable<TotalUsersDTO>> GetTotalUsersPerMonth(int year)
        {
            var param = new { Year = year};
            var totalUsersPerMonth = await RunSqlQuery<TotalUsersDTO>("sp_getTotalUsersPerMonth", param, commandType: CommandType.StoredProcedure);
            return totalUsersPerMonth;
        }

        // Get total orders per month
        public async Task<IQueryable<TotalOrdersDTO>> GetTotalOrdersPerMonth(int year)
        {
            var param = new { Year = year };
            var totalOrdersPerMonth = await RunSqlQuery<TotalOrdersDTO>("sp_getOrdersPerMonth", param, commandType: CommandType.StoredProcedure);
            return totalOrdersPerMonth;
        }

        // Get category list:
        public async Task<IQueryable<Sports>> GetCategory()
        {
            var query = "select * from Sports order by id";
            var sports = await RunSqlQuery<Sports>(query);
            return sports;
        }


        // Get sub category List
        public async Task<IQueryable<equipment_and_clothing>> GetSubCategory(int id)
        {
            var parameters = new { CategoryId = id };
            var query = "select * from equipment_and_clothing where sport_id = @CategoryId";
            var records = await RunSqlQuery<equipment_and_clothing>(query,parameters);
            return records;
        }


        // Get product List
        public async Task<IQueryable<Product>> GetProduct(int id)
        {
            var parameters = new { SubCategoryId= id };
            var query = "select * from product where equipment_id = @SubCategoryId";
            var records = await RunSqlQuery<Product>(query, parameters);
            return records;
        }

        // Get products list by Sport Id 
        public async Task<IQueryable<Product>> GetProductsBySportId(int id)
        {
            var parameters = new { SportId = id };
            var query = "select * from product where equipment_id in (select id from equipment_and_clothing where sport_id = @SportId)";
            var products = await RunSqlQuery<Product>(query,parameters);
            return products;
        }

        // Get all products details:
        public async Task<IQueryable<Product>> GetAllProducts()
        {
            var query = "select * from product";
            var products = await RunSqlQuery<Product>(query);
            return products;
        }

        // Get products' image Url list by Product Id 
        public async Task<string> GetProductImageUrl(int id)
        {
            var parameter = new { ProductId = id };
            var query = "select image_url from product where  id = @ProductId";
            var connection = _context.CreateConnection();
            var res = await connection.QueryFirstOrDefaultAsync<string>(query, parameter);
            return res;
        }


        //Get a product by it's Id :
        public async Task<IQueryable<Product>> GetProductByProductId(int id)
        {
            var parameter = new { ProductId = id };
            var query = "select * from product where id=@ProductId";
            var product = await RunSqlQuery<Product>(query, parameter);
            return product;
        }

        //Get individual User Using Token : 
        public async Task<RegisterList> GetUser()    
        {
            var username= _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;       // getting username from token

            RegisterList registerList = new RegisterList();
            var query = "SELECT * FROM Users WHERE username = @username";
            var user = await RunSqlQuery<RegisterDTO>(query,new { username = username });
            if (user.Count() > 0)
            {
                registerList.StatusCode = HttpStatusCode.OK;
                registerList.StatusMessage = "Successful";
                registerList.registers = user;
            }
            else
            { 
                registerList.StatusCode = HttpStatusCode.NotFound;
                registerList.StatusMessage = "User Not Found";
            }
            return registerList;
        }



        // Change Password :
        public async Task<DbResponse> ChangePassword(PasswordChangeDTO passwords)
        {
            var username = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@username", username);
                parameters.Add("@oldPassword", passwords.oldPassword);
                parameters.Add("@newPassword",passwords.newPassword);
                DbResponse result = (await connection.QueryAsync<DbResponse>("change_password", parameters)).FirstOrDefault();
                return result;
            }
        }






        // Adding User 
        public async Task<DbResponseWithToken> AddUser(Users user)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);
                parameters.Add("Password", user.Password);
                parameters.Add("Email", user.Email);
                parameters.Add("Firstname", user.First_Name);
                parameters.Add("Middlename", user.Middle_Name);
                parameters.Add("Lastname", user.Last_Name);
                parameters.Add("Address",user.Address);
                parameters.Add("Phone",user.Phone);
                parameters.Add("Role", user.Role);
                var token = _authService.GenerateJwtToken(user);
                var res = await connection.QueryAsync<DbResponseWithToken>("sp_register", parameters, commandType: CommandType.StoredProcedure);
                var result = res.ToList().FirstOrDefault();  //[ { Code = "000", Message = "User inserted successfully." } ] returns first object from list
                result.Token = token;
                return result;
            }
        }

        //Adding category
        public async Task<DbResponse> AddCategory(string name)
        {
                var param = new DynamicParameters();
                param.Add("name", name);
                var result = await RunSqlQuery<DbResponse>("sp_addcategory", param, commandType: CommandType.StoredProcedure);
                return result.ToList().FirstOrDefault();
        }

        //Adding Sub Category
        public async Task<DbResponse> AddSubCategory(equipment_and_clothing data)
        {
            var param = new DynamicParameters();
            param.Add("name", data.Name);
            param.Add("sport_id", data.sport_id);
            return await ExecuteMyCommand("sp_addSubCategory",param, commandType: CommandType.StoredProcedure);
        }

        //Adding Product
        public async Task<DbResponse> AddProduct(Product data)
        {
            var param = new DynamicParameters();
            param.Add("name", data.Name);
            param.Add("equipment_id", data.Equipment_id);
            param.Add("price",data.Price);
            param.Add("description",data.Description);
            return await ExecuteMyCommand("sp_addProduct", param, commandType: CommandType.StoredProcedure);
        }

        // Inserting order record:
        public async Task<DbResponse> InsertOrders(OrderDTO orderData)
        {
            var param = new DynamicParameters();
            param.Add("Username", orderData.Username); 
            param.Add("ProductId", orderData.ProductId);
            param.Add("OrderDate",orderData.OrderDate);
            param.Add("Quantity",orderData.Quantity);
            return await ExecuteMyCommand("sp_insert_order",param,commandType: CommandType.StoredProcedure);
        }


        // Add Ids to Cart:
        public async Task<DbResponse> AddToCart(CartDetails cartDetails)
        {
            var param = new DynamicParameters();
            param.Add("Username",cartDetails.Username);
            param.Add("ProductId",cartDetails.Product_Id);
            return await ExecuteMyCommand("sp_add_to_cart", param, commandType: CommandType.StoredProcedure);
        }

        //Delete Category 
        public async Task<DbResponse> DeleteCategory(int id)
        {
            var query = "DELETE FROM sports WHERE ID = @CategoryId";
            return await ExecuteMyCommand(query, new {CategoryId = id} );
        }
        
        
        //Delete Sub Category 
        public async Task<DbResponse> DeleteSubCategory(int id)
        {
            var query = "DELETE FROM equipment_and_clothing WHERE ID = @SubCategoryId";
            return await ExecuteMyCommand(query, new { SubCategoryId = id });
        }

        //Delete Product 
        public async Task<DbResponse> DeleteProduct(int id)
        {
            var query = "DELETE FROM product WHERE ID = @ProductId";
            return await ExecuteMyCommand(query, new { ProductId = id });
        }


        // Update Category
        public async Task<DbResponse> UpdateCategory(Sports product)
        {
            var param = new DynamicParameters();
            param.Add("sportId", product.Id);
            param.Add("name", product.Name);
            var query = "UPDATE sports SET name = @name WHERE id = @sportId"; 
            return await ExecuteMyCommand(query, param);
        }

        // Update Sub Category
        public async Task<DbResponse> UpdateSubCategory(equipment_and_clothing data)
        {
            var param = new DynamicParameters();
            param.Add("Id", data.ID);
            param.Add("name", data.Name);
            var query = "UPDATE equipment_and_clothing SET name = @name WHERE id = @Id";
            return await ExecuteMyCommand(query, param);
        }

        // Update Product
        public async Task<DbResponse> UpdateProduct(Product data)
        {
            var param = new DynamicParameters();
            param.Add("Id", data.Id);
            param.Add("name", data.Name);
            param.Add("description", data.Description);
            param.Add("price",data.Price);
            var query = "UPDATE product SET name = @name,price = @price,description = @description  WHERE id = @Id";
            return await ExecuteMyCommand(query, param);
        }

        //Login Functionality:
        public async Task<RegisterList> Authenticate(LoginDTO user)
        {
            RegisterList registerList = new RegisterList();
            var query = "SELECT * FROM Users WHERE username = @username AND password = @password";
            var parameters = new DynamicParameters();
            parameters.Add("username", user.Username);
            parameters.Add("password", user.Password);
            var authorized_user = await RunSqlQuery<RegisterDTO>(query, parameters);


            if (authorized_user.Any())
            {
                var token = _authService.GenerateJwtToken(authorized_user.FirstOrDefault());          //Generating Token after using username and password entered 

                var user_result = authorized_user.FirstOrDefault();
                var authorized_username = user_result.Username;

                var role = await GetRole(authorized_username);   //getting role from authorized user


                registerList.StatusCode = HttpStatusCode.OK;
                registerList.StatusMessage = "Authorized";
                registerList.Token = token;
                registerList.registers = authorized_user;
                registerList.role = role;

            }
            else
            {
                registerList.StatusCode = HttpStatusCode.Unauthorized;
                registerList.StatusMessage = "Unauthorized User";
            }

            return registerList;
        }




        // Get user role
        private async Task<string> GetRole(string username)
        {
            var get_role = "SELECT role from Users WHERE username = @username";
            var param = new DynamicParameters();
            param.Add("username", username);
            var result = await RunSqlQuery<GetRole>(get_role,param);
            var role = result.FirstOrDefault().role;
            return role;
        }

        // Get items added to cart by a username :
        public async Task<IQueryable> GetItemsAddedToCart(string username)
        {
            var param = new DynamicParameters();
            param.Add("username", username);
            var query = "select productId from orders where username = @username and status='pending' ";
            var result = await ReturnListFromDB(query, param);
            return result;
        }

        // Get Order by a username For User:
        public async Task<IQueryable> GetOrderByUsername(string username)
        {
            var param = new DynamicParameters();
            param.Add("username", username);
            var result = await RunSqlQuery<OrderDisplayDTO>("sp_display_orderDTO", param, commandType: CommandType.StoredProcedure);
            return result;
        }

        // Get All Orders by a username:
        public async Task<IQueryable<OrderHistoryDTO>> GetAllOrdersByUsername(string username)
        {
            var param = new DynamicParameters();
            param.Add("username", username);
            var query = "SELECT o.Orderdate, p.name AS product, o.Quantity,  o.price,  o.totalPrice,  o.status FROM orders o JOIN product p ON o.ProductId = p.id  where username=@username ORDER BY CASE WHEN o.status = 'pending' THEN 1 WHEN o.status = 'confirm' THEN 2 WHEN o.status = 'delivered' THEN 3   ELSE 4 END, o.Orderdate DESC;";
            var result = await RunSqlQuery<OrderHistoryDTO>(query,param);
            return result;
        }

        //Increase Quantity in order:
        public async Task<DbResponse> IncreaseQty(ChangeQtyDTO dtoDetails)
        {
            var param = new DynamicParameters();
            param.Add("Username", dtoDetails.Username);
            param.Add("ProductId",dtoDetails.ProductId);
            return  await ExecuteMyCommand("sp_increase_qty", param, commandType: CommandType.StoredProcedure);
        }


        //Decrease Quantity in order:
        public async Task<DbResponse> DecreaseQty(ChangeQtyDTO dtoDetails)
        {
            var param = new DynamicParameters();
            param.Add("Username", dtoDetails.Username);
            param.Add("ProductId", dtoDetails.ProductId);
            return await ExecuteMyCommand("sp_decrease_qty", param, commandType: CommandType.StoredProcedure);
        }

        //Delete Product from My Cart:
        public async Task<DbResponse> DeleteFromCart(ChangeQtyDTO dtoDetails)
        {
            var param = new DynamicParameters();
            param.Add("Username", dtoDetails.Username);
            param.Add("ProductId", dtoDetails.ProductId);
            return await ExecuteMyCommand("sp_delete_from_cart", param, commandType: CommandType.StoredProcedure);
        }


        //Confirming order from User:
        public async Task<DbResponse> ConfirmOrder(string username)
        {
            var param = new DynamicParameters();
            var query = "update orders set status='confirm' where username = @username and status='pending'";
            param.Add("username", username);
            var result = await ExecuteMyCommand(query, param);
            return result;
        }

        //Getting all orders that are confirmed:
        public async Task<IQueryable<ConfirmedOrders>> GetConfirmedOrders()
        {
            var query = "SELECT o.OrderId, o.Orderdate, o.username, p.name AS product, o.Quantity,  o.price,  o.totalPrice,  o.status FROM orders o JOIN product p ON o.ProductId = p.id WHERE o.status IN ('confirm', 'delivered') ORDER BY CASE WHEN o.status = 'confirm' THEN 1 WHEN o.status = 'delivered' THEN 2  ELSE 3 END, o.Orderdate DESC";
            var result = await RunSqlQuery<ConfirmedOrders>(query);
            return result;
        }

        //Make confirmed orders delivered:
        public async Task<DbResponse> MakeDelivery(int orderId)
        {
            var param = new DynamicParameters();
            param.Add("orderId",orderId);
            var query = "update orders set status = 'delivered' where orderId=@orderId";
            var result = await ExecuteMyCommand(query,param);
            return result;
        }

        // Methods to connect db and query

        //return only list :
        private async Task<IQueryable> ReturnListFromDB(string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var connection = _context.CreateConnection();
            return (await connection.QueryAsync(sql, parameters, commandType: commandType)).AsQueryable();
        }

        //return list of objects of particular model:
        private async Task<IQueryable<T>> RunSqlQuery<T>(string sql, object parameters = null,CommandType commandType = CommandType.Text)
        {
            var connection = _context.CreateConnection();
                return (await connection.QueryAsync<T>(sql,parameters,commandType: commandType)).AsQueryable();
        }



        
        // Method for executing command and returning only rows affected query (add, update and delete)
        private async Task<DbResponse> ExecuteMyCommand(string query, object parameters, CommandType commandType = CommandType.Text )
        {
            var connection = _context.CreateConnection();
            var response = await connection.QuerySingleOrDefaultAsync<DbResponse>(query, parameters, commandType: commandType);
            return response;
        }



        // Get category/sub category/ product by Id :
        public async Task<T> GetItem<T>(int id, string tableName)
        {
            var param = new DynamicParameters();
            param.Add("id", id);
            param.Add("tableName", tableName);
            var query = $"SELECT * FROM {tableName} where id = @id";
            var result = await RunSqlQuery<T>(query, param);
            return result.ToList().FirstOrDefault();
        } 
        
    }
}





