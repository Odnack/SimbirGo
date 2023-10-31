using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simbir.Go.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterTable_Rent_AddPriceOfUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PriceOfUnit",
                table: "Rents",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOfUnit",
                table: "Rents");
        }
    }
}
