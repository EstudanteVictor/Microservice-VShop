using System.ComponentModel.DataAnnotations;

namespace VShop.ProductApi.DTOs;

public class CategoryDTO
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage ="The name is required")]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }
}
