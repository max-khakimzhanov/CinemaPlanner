using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaPlanner.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieAgeRestriction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeRestriction",
                table: "Movies",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRestriction",
                table: "Movies");
        }
    }
}
