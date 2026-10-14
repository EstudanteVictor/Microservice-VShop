using VShop.web.Models;

namespace VShop.web.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProducts(string token);
        Task<ProductViewModel> FindProductById(string token,int id);
        Task<ProductViewModel> CreateProduct(string token,ProductViewModel productVM);
        Task<ProductViewModel> UpdateProduct(string token,ProductViewModel productVM);
        Task<bool> DeleteProductById(string token,int id);
    }
}
