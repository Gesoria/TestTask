using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFilter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLogsAndDeliveryOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CityDistrictId = table.Column<int>(type: "INTEGER", nullable: true),
                    StartFilterTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryOrders_CityDistricts_CityDistrictId",
                        column: x => x.CityDistrictId,
                        principalTable: "CityDistricts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeliveryOrdersWithOrders",
                columns: table => new
                {
                    DeliveryOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrdersId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrdersWithOrders", x => new { x.DeliveryOrderId, x.OrdersId });
                    table.ForeignKey(
                        name: "FK_DeliveryOrdersWithOrders_DeliveryOrders_DeliveryOrderId",
                        column: x => x.DeliveryOrderId,
                        principalTable: "DeliveryOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryOrdersWithOrders_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrders_CityDistrictId",
                table: "DeliveryOrders",
                column: "CityDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrdersWithOrders_OrdersId",
                table: "DeliveryOrdersWithOrders",
                column: "OrdersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryLogs");

            migrationBuilder.DropTable(
                name: "DeliveryOrdersWithOrders");

            migrationBuilder.DropTable(
                name: "DeliveryOrders");
        }
    }
}
