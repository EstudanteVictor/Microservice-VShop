using System.Text.Json;
using VShop.web.Models;
using VShop.web.Services.Contracts;

namespace VShop.web.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly JsonSerializerOptions _options;
        private const string apiEndPoint = "/api/categories";

        public CategoryService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllCategories(string token)
        {
            var client = _clientFactory.CreateClient("ProductAPI");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            IEnumerable<CategoryViewModel> categoriesVM;

            using (var response = await client.GetAsync(apiEndPoint))
            {

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    categoriesVM = await JsonSerializer.DeserializeAsync<IEnumerable<CategoryViewModel>>(apiResponse, _options);
                }
                else return null;

            return categoriesVM;
            }
        }
    }
}
