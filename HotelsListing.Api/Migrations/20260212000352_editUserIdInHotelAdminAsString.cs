using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelsListing.Api.Migrations
{
    /// <inheritdoc />
    public partial class editUserIdInHotelAdminAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelAdmins_AspNetUsers_UserId",
                table: "HotelAdmins");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "HotelAdmins");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "HotelAdmins",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_HotelAdmins_UserId",
                table: "HotelAdmins",
                newName: "IX_HotelAdmins_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelAdmins_AspNetUsers_UserID",
                table: "HotelAdmins",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelAdmins_AspNetUsers_UserID",
                table: "HotelAdmins");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "HotelAdmins",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HotelAdmins_UserID",
                table: "HotelAdmins",
                newName: "IX_HotelAdmins_UserId");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "HotelAdmins",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HotelAdmins_AspNetUsers_UserId",
                table: "HotelAdmins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
