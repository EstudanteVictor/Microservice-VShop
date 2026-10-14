using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VShop.web.Models;
using VShop.web.Roles;
using VShop.web.Services.Contracts;

namespace VShop.web.Controllers;

[Authorize(Roles = Role.Admin)]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> Index()
    {
        var result = await _productService.GetAllProducts(await GetAccessToken());

        if (result is null)
            return View("Error");

        return View(result);
    }

    [HttpGet]
    public async Task<ActionResult> CreateProduct()
    {
        ViewBag.CategoryId = new SelectList(await
            _categoryService.GetAllCategories(await GetAccessToken()), "CategoryId", "Name");

        return View();
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(ProductViewModel productVM)
    {
        if (ModelState.IsValid)
        {
            if (!productVM.ImageURL.Contains("escolar/"))
            {
                productVM.ImageURL = "escolar/" + productVM.ImageURL;
            }
            var result = await _productService.CreateProduct(await GetAccessToken(), productVM);

            if (result != null)
                return RedirectToAction(nameof(Index));
        }
        else
        {
            ViewBag.CategoryId = new SelectList(await
                _categoryService.GetAllCategories(await GetAccessToken()), "CategoryId", "Name");

        }
        return View(productVM);
    }

    [HttpGet] 
    public async Task<ActionResult> UpdateProduct(int id)
    {
        ViewBag.CategoryId = new SelectList(await
            _categoryService.GetAllCategories(await GetAccessToken()), "CategoryId", "Name");

        var result = await _productService.FindProductById(await GetAccessToken(), id);
        if (result is null)
            return View("Error");

        return View(result);
    }

    [HttpPost]
    public async Task<ActionResult> UpdateProduct(ProductViewModel productVM)
    {
        string? tokenJWT = await GetAccessToken();
        if (ModelState.IsValid)
        {
            if (!productVM.ImageURL.Contains("escolar/"))
            {
                productVM.ImageURL = "escolar/" + productVM.ImageURL;
            }

            var result = await _productService.UpdateProduct(await GetAccessToken(), productVM);

            if (result != null)
                return RedirectToAction(nameof(Index));
        }
        return View(productVM);
    }

    [HttpGet]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await _productService.FindProductById(await GetAccessToken(), id);

        if (result is null)
            return View("Error");

        return View(result);
    }

    [HttpPost(), ActionName("DeleteProduct")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _productService.DeleteProductById(await GetAccessToken(), id);

        if (!result)
        {
            return View("Error");
        }

        return RedirectToAction("Index");
    }

    private async Task<string?> GetAccessToken()
    {
        return await HttpContext.GetTokenAsync("access_token");
    }
}
