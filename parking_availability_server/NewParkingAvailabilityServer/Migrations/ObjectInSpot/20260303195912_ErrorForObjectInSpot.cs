using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewParkingAvailabilityServer.Migrations.ObjectInSpot
{
    /// <inheritdoc />
    public partial class ErrorForObjectInSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "error",
                table: "ObjectInSpotItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error",
                table: "ObjectInSpotItems");
        }
    }
}
