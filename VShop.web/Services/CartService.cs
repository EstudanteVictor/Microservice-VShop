using System.Text;
using System.Text.Json;
using VShop.web.Models;
using VShop.web.Services.Contracts;

namespace VShop.web.Services;

public class CartService : ICartService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions? _options;

    private const string apiEndPoint = "/api/cart/";
    private CartViewModel? cartVM = new();

    public CartService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
    }

    public async Task<CartViewModel> GetCartByUserIdAsync(string userId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.GetAsync($"{apiEndPoint}getcart/{userId}"))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartVM = await JsonSerializer.DeserializeAsync<CartViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartVM;
    }

    public async Task<CartViewModel> AddItemToCartAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent stringContent = new(JsonSerializer.Serialize(cartVM), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync($"{apiEndPoint}addcart/", stringContent))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartVM = await JsonSerializer.DeserializeAsync<CartViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartVM;
    }

    public async Task<CartViewModel> UpdateCartAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent stringContent = new(JsonSerializer.Serialize(cartVM), Encoding.UTF8, "application/json");
        CartViewModel cartUpdated = new();

        using (var response = await client.PutAsJsonAsync($"{apiEndPoint}updatecart/", stringContent))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                cartUpdated = await JsonSerializer.DeserializeAsync<CartViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        return cartUpdated;
    }

    public async Task<bool> DeleteItemCartAsync(int cartId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync($"{apiEndPoint}deletecart/{cartId}"))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// ////////////////////

    public Task<bool> CleanCartAsync(string userId, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ApplyCouponAsync(CartViewModel cartVM, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent stringContent = new(JsonSerializer.Serialize(cartVM), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync($"{apiEndPoint}applycoupon/", stringContent))
        {
            if (response.IsSuccessStatusCode)
                return true;
        }
        return false;
    }

    public async Task<bool> DeleteCouponAsync(string userId, string token)
    {
        var client = _clientFactory.CreateClient("CartApi");

        PutTokenInHeaderAuthorization(token, client);

        using (var response = await client.DeleteAsync($"{apiEndPoint}deletecoupon/{userId}"))
        {
            if (response.IsSuccessStatusCode)
                return true;
        }
        return false;
    }


    public Task<CartViewModel> CheckoutAsync(CartHeaderViewModel cartHeader, string token)
    {
        throw new NotImplementedException();
    }

    private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

}
