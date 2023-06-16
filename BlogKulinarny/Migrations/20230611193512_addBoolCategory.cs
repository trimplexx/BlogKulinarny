using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogKulinarny.Migrations
{
    /// <inheritdoc />
    public partial class addBoolCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAccepted",
                table: "categories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAccepted",
                table: "categories");
        }
    }
}
