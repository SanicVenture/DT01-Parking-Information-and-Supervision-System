using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewParkingAvailabilityServer.Migrations.OpenCVPolygons
{
    /// <inheritdoc />
    public partial class AddPolygons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenCVPolygonsItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    polygonPoints = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenCVPolygonsItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenCVPolygonsItems");
        }
    }
}
