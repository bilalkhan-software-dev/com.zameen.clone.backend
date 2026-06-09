using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace com.zameen.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalInfoInEnquiryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnic",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnquiryType",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonthlySalary",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Cnic",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "EnquiryType",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "MonthlySalary",
                table: "Enquiries");
        }
    }
}
