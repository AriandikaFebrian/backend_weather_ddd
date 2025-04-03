using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCa.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TodoItems",
                type: "bool",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TrackingId",
                table: "TodoItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Weathers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    WeatherDescription = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Humidity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    WindSpeed = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CloudCoverage = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    UpdatedBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    IsActive = table.Column<bool>(type: "bool", nullable: true, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bool", nullable: true, defaultValue: false),
                    TrackingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weathers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_IsDeleted",
                table: "TodoItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Weathers_IsActive",
                table: "Weathers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Weathers_IsDeleted",
                table: "Weathers",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Weathers");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_IsDeleted",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "TrackingId",
                table: "TodoItems");
        }
    }
}
