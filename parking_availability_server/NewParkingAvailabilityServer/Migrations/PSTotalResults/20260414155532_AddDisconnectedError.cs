using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewParkingAvailabilityServer.Migrations.PSTotalResults
{
    /// <inheritdoc />
    public partial class AddDisconnectedError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "sensorConnectedToNetwork",
                table: "PSTotalResultsItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sensorConnectedToNetwork",
                table: "PSTotalResultsItems");
        }
    }
}
