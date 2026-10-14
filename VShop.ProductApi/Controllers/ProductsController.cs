using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.ProductApi.DTO;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Roles;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    public readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll()
    {
        var productsDto = await _productService.GetProducts();
        return Ok(productsDto);
    }

    [HttpGet("{id:int}", Name ="ProductById")]
    public async Task<ActionResult<ProductDTO>> GetById(int id) 
    {
        var productFound = await _productService.GetProductById(id);
        if (productFound is null)
            return NotFound();

        return Ok(productFound);
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Post([FromBody] ProductDTO productDto) 
    {
        if (productDto is null)
            return BadRequest();

       await _productService.AddProduct(productDto);
       return new CreatedAtRouteResult("ProductById", new { id = productDto.Id }, productDto);
    }

    [HttpPut]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Put([FromBody] ProductDTO productDto)
    {

        if (productDto is null)
            return NotFound();

        await _productService.UpdateProduct(productDto);
        return Ok(productDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> Delete(int id)
    {
        var productFound = await _productService.GetProductById(id);

        if (productFound is null)
            return NotFound();

        await _productService.RemoveProduct(id);
        return NoContent();
    }
}
