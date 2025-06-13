using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiShop.Cargo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class revisedCargoCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoCustomers_CargoDetails_CargoDetailId",
                table: "CargoCustomers");

            migrationBuilder.DropIndex(
                name: "IX_CargoCustomers_CargoDetailId",
                table: "CargoCustomers");

            migrationBuilder.DropColumn(
                name: "CargoDetailId",
                table: "CargoCustomers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CargoDetailId",
                table: "CargoCustomers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CargoCustomers_CargoDetailId",
                table: "CargoCustomers",
                column: "CargoDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_CargoCustomers_CargoDetails_CargoDetailId",
                table: "CargoCustomers",
                column: "CargoDetailId",
                principalTable: "CargoDetails",
                principalColumn: "CargoDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
