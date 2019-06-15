using Microsoft.EntityFrameworkCore.Migrations;

namespace Stuart_Hopwood_Photography_API.Migrations
{
    public partial class remove_authtoken_unused_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "id_token",
                table: "OAuthToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "OAuthToken",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_token",
                table: "OAuthToken",
                nullable: true);
        }
    }
}
