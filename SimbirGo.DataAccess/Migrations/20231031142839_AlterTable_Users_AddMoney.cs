using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simbir.Go.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterTable_Users_AddMoney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Money",
                table: "Users",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Money",
                table: "Users");
        }
    }
}
