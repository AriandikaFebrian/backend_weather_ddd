// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCa.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Changelogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Method = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: true),
                TableName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                KeyValues = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                NewValues = table.Column<string>(type: "text", nullable: true),
                OldValues = table.Column<string>(type: "text", nullable: true),
                ChangeBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                ChangeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Changelogs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "MessageBroker",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Topic = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                Message = table.Column<string>(type: "text", nullable: true),
                StoredDate = table.Column<DateTime>(type: "Timestamp", nullable: true),
                IsSend = table.Column<bool>(type: "bool", nullable: false),
                Acknowledged = table.Column<bool>(type: "bool", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MessageBroker", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ReceivedMessageBroker",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Topic = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                Message = table.Column<string>(type: "text", nullable: true),
                Error = table.Column<string>(type: "text", nullable: true),
                TimeIn = table.Column<DateTime>(type: "timestamp", nullable: false),
                Offset = table.Column<long>(type: "bigint", nullable: false),
                Partition = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: true),
                InnerMessage = table.Column<string>(type: "text", nullable: true),
                StackTrace = table.Column<string>(type: "text", nullable: true),
                TimeProcess = table.Column<DateTime>(type: "timestamp", nullable: true),
                TimeFinish = table.Column<DateTime>(type: "timestamp", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReceivedMessageBroker", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TodoItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                CreatedBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                UpdatedBy = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                IsActive = table.Column<bool>(type: "bool", nullable: true, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TodoItems", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TodoItems_IsActive",
            table: "TodoItems",
            column: "IsActive");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Changelogs");

        migrationBuilder.DropTable(
            name: "MessageBroker");

        migrationBuilder.DropTable(
            name: "ReceivedMessageBroker");

        migrationBuilder.DropTable(
            name: "TodoItems");
    }
}
