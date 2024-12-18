using Ecommerce.DTO;
using Ecommerce.Entities;

namespace Ecommerce.Repository
{ 
    public interface ICompanyRepository
    {
        Task<IQueryable<RegisterDTO>> GetUsers();
        Task<IQueryable<Users>> GetUsersForAdmin();
        Task<RegisterList> GetUser();
        Task<DbResponseWithToken> AddUser(Users user); 
        Task<RegisterList> Authenticate(LoginDTO user);
        Task<DbResponse> ChangePassword(PasswordChangeDTO passwords);
        Task<IQueryable<Sports>> GetCategory();
        Task<IQueryable<equipment_and_clothing>> GetSubCategory(int id);
        Task<IQueryable<Product>> GetProduct(int id);
        Task<IQueryable<Product>> GetProductsBySportId(int id);
        Task<IQueryable<Product>> GetProductByProductId(int id);
        Task<DbResponse> AddCategory(string name);
        Task<DbResponse> DeleteCategory(int id);
        Task<T> GetItem<T>(int id, string tableName);
        Task<DbResponse> UpdateCategory(Sports product);
        Task<DbResponse> AddSubCategory(equipment_and_clothing data);
        Task<DbResponse> AddProduct(Product data);
        Task<DbResponse> InsertOrders(OrderDTO orderData);
        Task<DbResponse> DeleteSubCategory(int id);
        Task<DbResponse> DeleteProduct(int id);
        Task<DbResponse> UpdateSubCategory(equipment_and_clothing data);
        Task<DbResponse> UpdateProduct(Product data);
        Task<IQueryable<TotalUsersDTO>> GetTotalUsersPerMonth(int year);
        Task<IQueryable> GetItemsAddedToCart(string username);
        Task<IQueryable> GetOrderByUsername(string username);
        Task<DbResponse> AddToCart(CartDetails cartDetails);
        Task<DbResponse> IncreaseQty(ChangeQtyDTO dtoDetails);
        Task<DbResponse> DecreaseQty(ChangeQtyDTO dtoDetails);
        Task<DbResponse> DeleteFromCart(ChangeQtyDTO dtoDetails);
        Task<string> GetProductImageUrl(int id);
        Task<IQueryable<Product>> GetAllProducts();
        Task<IQueryable<TotalOrdersDTO>> GetTotalOrdersPerMonth(int year);
        Task<DbResponse> ConfirmOrder(string username);
        Task<IQueryable<ConfirmedOrders>> GetConfirmedOrders();
        Task<DbResponse> MakeDelivery(int orderId);
        Task<IQueryable<OrderHistoryDTO>> GetAllOrdersByUsername(string username);
    };
}
