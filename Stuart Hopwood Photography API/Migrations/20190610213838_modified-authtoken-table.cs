using Microsoft.EntityFrameworkCore.Migrations;

namespace Stuart_Hopwood_Photography_API.Migrations
{
    public partial class modifiedauthtokentable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Issued",
                table: "OAuthToken",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedUtc",
                table: "OAuthToken",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "OAuthToken",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "expires_in",
                table: "OAuthToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "id_token",
                table: "OAuthToken",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "OAuthToken",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "scope",
                table: "OAuthToken",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token_type",
                table: "OAuthToken",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issued",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "IssuedUtc",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "access_token",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "expires_in",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "id_token",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "scope",
                table: "OAuthToken");

            migrationBuilder.DropColumn(
                name: "token_type",
                table: "OAuthToken");
        }
    }
}
