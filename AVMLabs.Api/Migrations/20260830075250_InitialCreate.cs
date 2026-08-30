using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AVMLabs.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SampleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.TestId);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    WOId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    WODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.WOId);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderItems",
                columns: table => new
                {
                    WOItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WOId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderItems", x => x.WOItemId);
                    table.ForeignKey(
                        name: "FK_WorkOrderItems_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderItems_WorkOrders_WOId",
                        column: x => x.WOId,
                        principalTable: "WorkOrders",
                        principalColumn: "WOId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "City", "ClientName", "ContactPerson", "Country", "Email", "IsActive", "Phone" },
                values: new object[,]
                {
                    { 1, "Dubai", "Al Noor Hospital", "Ahmed Khalid", "UAE", "ahmed@alnoor.ae", true, "+971500000001" },
                    { 2, "Chennai", "Apollo Diagnostics", "Priya Menon", "India", "priya@apollo.in", true, "+919840000002" },
                    { 3, "Doha", "Gulf Care Clinic", "Sara Ali", "Qatar", "sara@gulfcare.qa", true, "+96550000003" }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "TestId", "IsActive", "Rate", "SampleType", "TestCode", "TestName" },
                values: new object[,]
                {
                    { 1, true, 15.00m, "Blood", "CBC001", "Complete Blood Count" },
                    { 2, true, 25.00m, "Blood", "LFT001", "Liver Function Test" },
                    { 3, true, 25.00m, "Blood", "KFT001", "Kidney Function Test" },
                    { 4, true, 10.00m, "Urine", "URN001", "Urine Routine" },
                    { 5, true, 30.00m, "Blood", "THY001", "Thyroid Profile" }
                });

            migrationBuilder.InsertData(
                table: "WorkOrders",
                columns: new[] { "WOId", "ClientId", "Status", "TotalAmount", "WODate" },
                values: new object[,]
                {
                    { 1, 1, "Completed", 40.00m, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "Pending", 35.00m, new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, "Completed", 30.00m, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "WorkOrderItems",
                columns: new[] { "WOItemId", "Quantity", "Rate", "TestId", "WOId" },
                values: new object[,]
                {
                    { 1, 1, 15.00m, 1, 1 },
                    { 2, 1, 25.00m, 2, 1 },
                    { 3, 1, 25.00m, 3, 2 },
                    { 4, 1, 10.00m, 4, 2 },
                    { 5, 1, 30.00m, 5, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderItems_TestId",
                table: "WorkOrderItems",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderItems_WOId",
                table: "WorkOrderItems",
                column: "WOId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ClientId",
                table: "WorkOrders",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "WorkOrderItems");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
