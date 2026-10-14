using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.Json;
using VShop.web.Models;
using VShop.web.Services.Contracts;

namespace VShop.web.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private const string apiEndPoint = "/api/products/";
    private readonly JsonSerializerOptions _options;
    private ProductViewModel productVM;
    private IEnumerable<ProductViewModel> productsVM;

    public ProductService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts(string token)
    {
        var client = _clientFactory.CreateClient("ProductAPI");
        PutTokenInHeaderAuthorization(token, client);
        using (var response = await client.GetAsync(apiEndPoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productsVM = await JsonSerializer.DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return productsVM;
    }

    public async Task<ProductViewModel> FindProductById(string token,int id)
    {
        var client = _clientFactory.CreateClient("ProductAPI");
        PutTokenInHeaderAuthorization(token, client);
        var productVM = new ProductViewModel();
        using (var response = await client.GetAsync($"{apiEndPoint}{id}"))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return productVM;
    }

    public async Task<ProductViewModel> CreateProduct(string token,ProductViewModel productVM)
    {
        var client = _clientFactory.CreateClient("ProductAPI");
        PutTokenInHeaderAuthorization(token, client);
        var JsonResponse = new StringContent(JsonSerializer.Serialize(productVM), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndPoint, JsonResponse))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else return null;
        }
        return productVM;
    }

    public async Task<ProductViewModel> UpdateProduct(string token,ProductViewModel productVM)
    {
        var client = _clientFactory.CreateClient("ProductAPI");
        PutTokenInHeaderAuthorization(token, client);

        var JsonResponse = new StringContent(JsonSerializer.Serialize(productVM), Encoding.UTF8, "application/json");
        ProductViewModel productUpdated = new ProductViewModel();

        using (var response = await client.PutAsync(apiEndPoint, JsonResponse))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productUpdated = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else return null;
        }
        return productUpdated;
    }

    public async Task<bool> DeleteProductById(string token,int id)
    {
        var client = _clientFactory.CreateClient("ProductAPI");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync(apiEndPoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
