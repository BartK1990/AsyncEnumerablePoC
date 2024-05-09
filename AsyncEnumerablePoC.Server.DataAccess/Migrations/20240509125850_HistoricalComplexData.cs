using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsyncEnumerablePoC.Server.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class HistoricalComplexData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricalComplexData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value1 = table.Column<double>(type: "float", nullable: false),
                    Value2 = table.Column<double>(type: "float", nullable: false),
                    Value3 = table.Column<double>(type: "float", nullable: false),
                    Value4 = table.Column<double>(type: "float", nullable: false),
                    Value5 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalComplexData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalComplexData");
        }
    }
}
