using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Models;
using VShop.ProductApi.Roles;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categorieService;

        public CategoriesController(ICategoryService categorieService)
        {
            _categorieService = categorieService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var categoriesDto = await _categorieService.GetCategories();
            if (categoriesDto is null)
                return NotFound();
            return Ok(categoriesDto);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetProducts()
        {
            var categoryDto = await _categorieService.GetCategoriesProducts();
            if (categoryDto is null)
                return NotFound();
            return Ok(categoryDto);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var categoryDto = await _categorieService.GetCategoryById(id);
            if (categoryDto is not null)
                return NotFound();
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)


                await _categorieService.AddCategory(categoryDTO);
            //return Created("categories/{id}", categoryDTO.CategoryId);

            return new CreatedAtRouteResult("GetCategory", new { id = categoryDTO.CategoryId }, categoryDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (categoryDTO is null)
                return BadRequest();
            if (categoryDTO.CategoryId != id)
                return BadRequest();

            await _categorieService.UpdateCategory(categoryDTO);
            return Ok(categoryDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult> Delete(int id)
        {
            var categoryDeleted = _categorieService.GetCategoryById(id);

            if (categoryDeleted is null)
                return NotFound();

            await _categorieService.RemoveCategory(id);
            return NoContent();
        }
    }
}
