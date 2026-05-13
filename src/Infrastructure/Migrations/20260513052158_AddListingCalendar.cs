using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heven.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListingCalendars",
                columns: table => new
                {
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingCalendars", x => new { x.ListingId, x.Date });
                    table.CheckConstraint("CK_ListingCalendar_Status", "Status IN ('available', 'blocked', 'booked')");
                    table.ForeignKey(
                        name: "FK_ListingCalendars_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ListingCalendars_Listings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_listing_calendar_reservation",
                table: "ListingCalendars",
                column: "BookingId",
                filter: "BookingId IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_listing_calendar_status",
                table: "ListingCalendars",
                columns: new[] { "ListingId", "Status", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListingCalendars");
        }
    }
}
