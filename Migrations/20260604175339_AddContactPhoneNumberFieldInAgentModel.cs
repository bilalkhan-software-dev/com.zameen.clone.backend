using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.zameen.Migrations
{
    /// <inheritdoc />
    public partial class AddContactPhoneNumberFieldInAgentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Agents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Agents");
        }
    }
}
