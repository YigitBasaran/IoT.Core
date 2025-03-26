using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IoT.Core.AuthService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 0, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), false, "$2a$11$938geM.vJ0pq5raKUfMBB.y0Wq6MSuIefVoptpY5kFEVI4Palx2WC", 0, null, "admin" },
                    { 1, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), false, "$2a$11$djNSsxIQuzCrU1AQktDX4uqysqnNLaX9OhLDdq5ZUIXW7qWXPC2na", 1, null, "Tesla" },
                    { 2, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), false, "$2a$11$Gw1V.HcCVJamgzWmKyzO6O/8WoSJrzK5oO9gAthNWTXLt3iaE2GHq", 1, null, "Google" },
                    { 3, new DateTime(2024, 3, 20, 12, 0, 0, 0, DateTimeKind.Utc), false, "$2a$11$nImEvlk9fjvOLY9Qkqkdm.MAbvaH1li6jpm3J7jl1n5EHHzWa8xSK", 1, null, "Amazon" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
