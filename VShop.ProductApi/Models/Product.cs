namespace VShop.ProductApi.Models;
public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Discription { get; set; }
    public long Stock { get; set; }
    public string? ImageURL { get; set; }

    //Navigation property for the Category
    public Category? Category { get; set; }
    public int CategoryId { get; set; }
}
