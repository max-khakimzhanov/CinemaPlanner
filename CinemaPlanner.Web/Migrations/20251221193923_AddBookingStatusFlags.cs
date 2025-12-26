using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaPlanner.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusFlags",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusFlags",
                table: "Bookings");
        }
    }
}
