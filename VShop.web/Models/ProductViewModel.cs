using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VShop.web.Models;

public class ProductViewModel
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public string? Discription { get; set; }
    [Required]
    public long Stock { get; set; }
    public string? ImageURL { get; set; }
    [Range(1,100)]
    public int Quantity { get; set; } = 1;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
