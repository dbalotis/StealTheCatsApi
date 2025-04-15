using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StealTheCatsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToCatIdAndTagName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cats_CatId",
                table: "Cats",
                column: "CatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Cats_CatId",
                table: "Cats");
        }
    }
}
