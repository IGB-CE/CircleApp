using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleApp.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Posts_PostId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Posts_PostId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Posts_PostId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Report");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Report",
                newName: "IX_Report_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Report_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Report",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Report_PostId",
                table: "Favorites",
                column: "PostId",
                principalTable: "Report",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Report_PostId",
                table: "Likes",
                column: "PostId",
                principalTable: "Report",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Users_UserId",
                table: "Report",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Report_PostId",
                table: "Reports",
                column: "PostId",
                principalTable: "Report",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Report_PostId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Report_PostId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Report_PostId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Users_UserId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Report_PostId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Posts");

            migrationBuilder.RenameIndex(
                name: "IX_Report_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Posts_PostId",
                table: "Favorites",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Posts_PostId",
                table: "Likes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Posts_PostId",
                table: "Reports",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
