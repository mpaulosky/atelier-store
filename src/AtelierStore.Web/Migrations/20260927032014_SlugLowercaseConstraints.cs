using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtelierStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class SlugLowercaseConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "ck_products_slug_lowercase",
                table: "products",
                sql: "slug = lower(slug)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categories_slug_lowercase",
                table: "categories",
                sql: "slug = lower(slug)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_products_slug_lowercase",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categories_slug_lowercase",
                table: "categories");
        }
    }
}
