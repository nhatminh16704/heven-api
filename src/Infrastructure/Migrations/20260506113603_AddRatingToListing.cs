using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heven.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "ListingAmenities");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ListingAmenities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ListingAmenities");

            migrationBuilder.AddColumn<double>(
                name: "RatingAverage",
                table: "Listings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmenityId",
                table: "ListingAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListingAmenities_AmenityId",
                table: "ListingAmenities",
                column: "AmenityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAmenities_Amenities_AmenityId",
                table: "ListingAmenities",
                column: "AmenityId",
                principalTable: "Amenities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingAmenities_Amenities_AmenityId",
                table: "ListingAmenities");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropIndex(
                name: "IX_ListingAmenities_AmenityId",
                table: "ListingAmenities");

            migrationBuilder.DropColumn(
                name: "RatingAverage",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "AmenityId",
                table: "ListingAmenities");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ListingAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ListingAmenities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ListingAmenities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
