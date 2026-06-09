using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.zameen.Migrations
{
    /// <inheritdoc />
    public partial class AddCityToPriceTrend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "PriceTrends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "PriceTrends");
        }
    }
}
