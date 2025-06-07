using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace socialApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesAndDpIdToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_DisplayPictures_DisplayPictureId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPictures_UserId",
                table: "DisplayPictures");

            migrationBuilder.RenameColumn(
                name: "DisplayPictureId",
                table: "AspNetUsers",
                newName: "CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DisplayPictureId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CommentId");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Comments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "DisplayPictureLikes",
                columns: table => new
                {
                    DisplayPicture1Id = table.Column<int>(type: "integer", nullable: false),
                    LikesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayPictureLikes", x => new { x.DisplayPicture1Id, x.LikesId });
                    table.ForeignKey(
                        name: "FK_DisplayPictureLikes_AspNetUsers_LikesId",
                        column: x => x.LikesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisplayPictureLikes_DisplayPictures_DisplayPicture1Id",
                        column: x => x.DisplayPicture1Id,
                        principalTable: "DisplayPictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPictures_UserId",
                table: "DisplayPictures",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPictureLikes_LikesId",
                table: "DisplayPictureLikes",
                column: "LikesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Comments_CommentId",
                table: "AspNetUsers",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Comments_CommentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "DisplayPictureLikes");

            migrationBuilder.DropIndex(
                name: "IX_DisplayPictures_UserId",
                table: "DisplayPictures");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "AspNetUsers",
                newName: "DisplayPictureId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CommentId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DisplayPictureId");

            migrationBuilder.AlterColumn<int>(
                name: "PostId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisplayPictures_UserId",
                table: "DisplayPictures",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_DisplayPictures_DisplayPictureId",
                table: "AspNetUsers",
                column: "DisplayPictureId",
                principalTable: "DisplayPictures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                table: "Comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
