using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class addHotelPerNightRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PerNightRate",
                table: "Hotels",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerNightRate",
                table: "Hotels");
        }
    }
}
