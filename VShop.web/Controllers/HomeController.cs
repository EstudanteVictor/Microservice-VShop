using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VShop.web.Models;
using VShop.web.Services.Contracts;

namespace VShop.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<HomeController> _logger;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _logger = logger;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAllProducts(string.Empty);

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult<ProductViewModel>> ProductDetails(int id)
        {
            var product = await _productService.FindProductById(string.Empty, id);
            if (product is null)
                return View("Error");

            return View(product);
        }

        [HttpPost]
        [ActionName("ProductDetails")]
        [Authorize]
        public async Task<ActionResult<ProductViewModel>> ProductDetailPost(ProductViewModel ProductVM)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            CartViewModel cartView = new()
            {
                CartHeader = new CartHeaderViewModel
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartItemViewModel cartItem = new()
            {
                Quantity = ProductVM.Quantity,
                ProductId = ProductVM.Id,
                Product = await _productService.FindProductById(accessToken,ProductVM.Id)
            };

            List<CartItemViewModel> cartItemsVM = new();
            cartItemsVM.Add(cartItem);
            cartView.CartItems = cartItemsVM;

            var result = await _cartService.AddItemToCartAsync(cartView, accessToken);

            if (result is not null)
                return RedirectToAction(nameof(Index));

            return View(ProductVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
