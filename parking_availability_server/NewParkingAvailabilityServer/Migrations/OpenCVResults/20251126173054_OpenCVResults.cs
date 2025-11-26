using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewParkingAvailabilityServer.Migrations.OpenCVResults
{
    /// <inheritdoc />
    public partial class OpenCVResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenCVResultsItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    vehicle = table.Column<bool>(type: "INTEGER", nullable: false),
                    parkingSpaceObstructed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCVResultsItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenCVResultsItems");
        }
    }
}
