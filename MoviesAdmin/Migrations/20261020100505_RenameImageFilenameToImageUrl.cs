using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RenameImageFilenameToImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFilename",
                table: "Movie",
                newName: "ImageUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Movie",
                newName: "ImageFilename");
        }

    }
}
