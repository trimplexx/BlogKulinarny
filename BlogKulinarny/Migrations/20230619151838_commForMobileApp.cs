using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogKulinarny.Migrations
{
    /// <inheritdoc />
    public partial class commForMobileApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBlocked",
                table: "comments",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBlocked",
                table: "comments");
        }
    }
}
