using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentsProcessorService.Infra.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentCreatedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    UserId = table.Column<int>(type: "INT", nullable: false),
                    GameId = table.Column<int>(type: "INT", nullable: false),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Currency = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCreatedEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatusChangedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    PaymentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    OldStatus = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    NewStatus = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    Observation = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatusChangedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentStatusChangedEvents_PaymentCreatedEvents_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "PaymentCreatedEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStatusChangedEvents_PaymentId",
                table: "PaymentStatusChangedEvents",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentStatusChangedEvents");

            migrationBuilder.DropTable(
                name: "PaymentCreatedEvents");
        }
    }
}
