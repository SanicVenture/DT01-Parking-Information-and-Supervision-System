using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewParkingAvailabilityServer.Migrations.PSTotalResults
{
    /// <inheritdoc />
    public partial class PSFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PSTotalResultsItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    vehicle = table.Column<bool>(type: "INTEGER", nullable: false),
                    objectInSpot = table.Column<bool>(type: "INTEGER", nullable: false),
                    parkingSpaceObstructed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSTotalResultsItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PSTotalResultsItems");
        }
    }
}
