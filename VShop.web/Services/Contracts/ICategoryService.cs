using VShop.web.Models;

namespace VShop.web.Services.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllCategories(string token);
    }
}
