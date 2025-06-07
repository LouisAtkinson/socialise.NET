using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace socialApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDisplayPictureModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Filename",
                table: "DisplayPictures");

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "DisplayPictures",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "DisplayPictures");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "DisplayPictures",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
