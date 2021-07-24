using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicTube.Repository.Migrations
{
    public partial class CoverArtAlbum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlbumCoverArt",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumCoverArt",
                table: "Albums");
        }
    }
}
