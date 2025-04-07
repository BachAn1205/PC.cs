using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Addresses",
                newName: "Ward");

            migrationBuilder.RenameColumn(
                name: "PinCode",
                table: "Addresses",
                newName: "District");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "Addresses",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "District",
                table: "Addresses",
                newName: "PinCode");
        }
    }
}
