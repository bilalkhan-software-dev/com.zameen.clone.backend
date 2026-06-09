using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.zameen.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyPurposeAndTypeInSearchLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyPurpose",
                table: "SearchLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "SearchLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyPurpose",
                table: "SearchLogs");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "SearchLogs");
        }
    }
}
