using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogKulinarny.Migrations
{
    /// <inheritdoc />
    public partial class commFixv4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_CommentId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comments_CommentId",
                table: "comments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id");
        }
    }
}
