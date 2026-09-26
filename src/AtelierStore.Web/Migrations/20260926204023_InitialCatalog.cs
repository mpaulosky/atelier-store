using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AtelierStore.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    was_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    image_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    badge = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.CheckConstraint("ck_products_price_non_negative", "price >= 0");
                    table.CheckConstraint("ck_products_was_price_above_price", "was_price IS NULL OR was_price > price");
                    table.ForeignKey(
                        name: "fk_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_stock",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_stock", x => x.product_id);
                    table.CheckConstraint("ck_product_stock_quantity_non_negative", "quantity >= 0");
                    table.ForeignKey(
                        name: "fk_product_stock_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "name", "slug" },
                values: new object[,]
                {
                    { 1, "Outerwear", "outerwear" },
                    { 2, "Knitwear", "knitwear" },
                    { 3, "Trousers", "trousers" },
                    { 4, "Handbags", "handbags" },
                    { 5, "Shoes", "shoes" },
                    { 6, "Eyewear", "eyewear" },
                    { 7, "Jewelry", "jewelry" }
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "badge", "category_id", "created_at", "description", "image_id", "name", "price", "slug", "was_price" },
                values: new object[,]
                {
                    { 1, "New", 1, new DateTimeOffset(new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Cut from supple lambskin with a slightly cropped body, asymmetric zip and quilted shoulders. Lined in cupro, so it layers easily over knitwear.", "1551028719-00167b16eac5", "Leather Biker Jacket", 2400m, "leather-biker-jacket", null },
                    { 2, null, 2, new DateTimeOffset(new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Knitted by hand in a soft wool and alpaca blend, finished with a long fringed hem. Drapes loosely from the shoulders and wears over coats or alone.", "1434389677669-e08b4cac3105", "Hand-Knit Fringe Poncho", 980m, "hand-knit-poncho", null },
                    { 3, null, 3, new DateTimeOffset(new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Washed silk twill with a drawstring waist and cuffed ankles. Relaxed through the leg, with deep side pockets and a fluid, matte finish.", "1594633312681-425c7b97ccd1", "Silk Jogger Trouser", 540m, "silk-jogger", 720m },
                    { 4, "Exclusive", 4, new DateTimeOffset(new DateTime(2026, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A structured calfskin bag with a rolled top handle, detachable shoulder strap and palladium hardware. The interior is lined in suede, with one zip pocket.", "1584917865442-de89df76afd3", "Structured Top-Handle Bag", 2950m, "top-handle-bag", null },
                    { 5, null, 4, new DateTimeOffset(new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "An open canvas tote printed with an archive botanical motif and trimmed in vegetable-tanned leather. Roomy enough for a laptop and a day's essentials.", "1591561954557-26941169b49e", "Botanical Print Tote", 1850m, "botanical-tote", null },
                    { 6, "New", 5, new DateTimeOffset(new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A retro runner built from suede and technical mesh panels on a lightweight rubber sole. Made in Italy and finished with a padded collar.", "1560769629-975ec94e6a86", "Panelled Runner Sneaker", 790m, "panelled-runner-sneaker", null },
                    { 7, null, 6, new DateTimeOffset(new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Fine round frames in brushed gold-tone metal with adjustable nose pads and tinted lenses offering full UV protection. Comes with a leather case.", "1511499767150-a48a237f0083", "Round Metal Sunglasses", 390m, "round-metal-sunglasses", null },
                    { 8, null, 7, new DateTimeOffset(new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Freshwater pearls, hand-knotted on silk and closed with an engraved gold vermeil clasp. Sits close at the base of the neck.", "1515562141207-7a88fb7ce338", "Pearl Collar Necklace", 1450m, "pearl-collar-necklace", null }
                });

            migrationBuilder.InsertData(
                table: "product_stock",
                columns: new[] { "product_id", "quantity", "updated_at" },
                values: new object[,]
                {
                    { 1, 6, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, 2, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, 9, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, 1, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, 0, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, 12, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 7, 7, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 8, 3, new DateTimeOffset(new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_categories_slug",
                table: "categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_slug",
                table: "products",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_stock");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
