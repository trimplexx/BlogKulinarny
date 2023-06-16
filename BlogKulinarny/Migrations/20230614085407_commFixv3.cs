using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogKulinarny.Migrations
{
    /// <inheritdoc />
    public partial class commFixv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_comments_CommentId",
                table: "comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_userId",
                table: "comments",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_users_userId",
                table: "comments",
                column: "userId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_CommentId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_users_userId",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_CommentId",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_userId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "comments");
        }
    }
}
