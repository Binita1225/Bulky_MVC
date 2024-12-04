using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPurchaseMasterToDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseMasters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseMasters");
        }
    }
}
