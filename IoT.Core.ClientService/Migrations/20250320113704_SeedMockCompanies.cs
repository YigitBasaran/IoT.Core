using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IoT.Core.ClientService.Migrations
{
    /// <inheritdoc />
    public partial class SeedMockCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "CreatedAt", "Email", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), "contact@tesla.com", false, "Tesla", null },
                    { 2, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), "info@google.com", false, "Google", null },
                    { 3, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), "support@amazon.com", false, "Amazon", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
