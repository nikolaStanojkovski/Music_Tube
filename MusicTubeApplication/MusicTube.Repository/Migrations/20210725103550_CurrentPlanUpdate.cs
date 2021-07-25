using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicTube.Repository.Migrations
{
    public partial class CurrentPlanUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromCurrentPlan",
                table: "Albums",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromCurrentPlan",
                table: "Albums");
        }
    }
}
