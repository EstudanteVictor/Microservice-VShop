using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.ProductApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Insert into Products(Name, Price, Discription, Stock, ImageURL, CategoryId) Values ('Caderno', 8.40, 'Caderno Espiral', 10, 'caderno1.jpg', 1)");
            migrationBuilder.Sql("Insert into Products(Name, Price, Discription, Stock, ImageURL, CategoryId) Values ('Lápis', 1.40, 'Lápis Preto', 10, 'lapis1.jpg', 2)");
            migrationBuilder.Sql("Insert into Products(Name, Price, Discription, Stock, ImageURL, CategoryId) Values ('Clips', 0.40, 'Clips para papel', 10, 'clips1.jpg', 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from Products");
        }
    }
}
