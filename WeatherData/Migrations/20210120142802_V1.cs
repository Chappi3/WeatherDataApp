using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WeatherData.Migrations
{
    public partial class V1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "temperatureReadings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Thermometer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_temperatureReadings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "temperatureReadings");
        }
    }
}
