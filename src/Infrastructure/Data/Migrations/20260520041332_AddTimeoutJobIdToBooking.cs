using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heven.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeoutJobIdToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeoutJobId",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeoutJobId",
                table: "Bookings");
        }
    }
}
