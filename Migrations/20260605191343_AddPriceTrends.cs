using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.zameen.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceTrends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Properties_AgentId",
                table: "Properties",
                newName: "IX_Property_AgentId");

            migrationBuilder.AlterColumn<string>(
                name: "PropertyPurpose",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Agents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceTrends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyPurpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SizeRange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTrends", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agent_UserId",
                table: "Agents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceTrends");

            migrationBuilder.DropIndex(
                name: "IX_Agent_UserId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Agents");

            migrationBuilder.RenameIndex(
                name: "IX_Property_AgentId",
                table: "Properties",
                newName: "IX_Properties_AgentId");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyPurpose",
                table: "Properties",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
