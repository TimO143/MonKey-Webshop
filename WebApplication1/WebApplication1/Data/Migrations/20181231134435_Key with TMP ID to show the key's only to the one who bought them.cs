using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication1.Data.Migrations
{
    public partial class KeywithTMPIDtoshowthekeysonlytotheonewhoboughtthem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TMPID",
                table: "Key",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TMPID",
                table: "Key");

            migrationBuilder.CreateTable(
                name: "Builder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Logo = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builder", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Factuur",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Building_nummer = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    E_mail = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderID = table.Column<int>(nullable: true),
                    Payed = table.Column<bool>(nullable: false),
                    Streetname = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Zip_Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factuur", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Factuur_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orderd_Product",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KeyID = table.Column<int>(nullable: true),
                    Key_ID = table.Column<int>(nullable: false),
                    OrderID = table.Column<int>(nullable: true),
                    Order_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orderd_Product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orderd_Product_Key_KeyID",
                        column: x => x.KeyID,
                        principalTable: "Key",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orderd_Product_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    User_role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Role_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Builder_Product",
                columns: table => new
                {
                    Builder_ID = table.Column<int>(nullable: false),
                    Product_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builder_Product", x => new { x.Builder_ID, x.Product_ID });
                    table.ForeignKey(
                        name: "FK_Builder_Product_Builder_Builder_ID",
                        column: x => x.Builder_ID,
                        principalTable: "Builder",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Builder_Product_Product_Product_ID",
                        column: x => x.Product_ID,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factuur_Producten",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FactuurID = table.Column<int>(nullable: true),
                    Factuur_ID = table.Column<int>(nullable: false),
                    KeyID = table.Column<int>(nullable: true),
                    Key_ID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factuur_Producten", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Factuur_Producten_Factuur_FactuurID",
                        column: x => x.FactuurID,
                        principalTable: "Factuur",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Factuur_Producten_Key_KeyID",
                        column: x => x.KeyID,
                        principalTable: "Key",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Builder_Product_Product_ID",
                table: "Builder_Product",
                column: "Product_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Factuur_OrderID",
                table: "Factuur",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Factuur_Producten_FactuurID",
                table: "Factuur_Producten",
                column: "FactuurID");

            migrationBuilder.CreateIndex(
                name: "IX_Factuur_Producten_KeyID",
                table: "Factuur_Producten",
                column: "KeyID");

            migrationBuilder.CreateIndex(
                name: "IX_Orderd_Product_KeyID",
                table: "Orderd_Product",
                column: "KeyID");

            migrationBuilder.CreateIndex(
                name: "IX_Orderd_Product_OrderID",
                table: "Orderd_Product",
                column: "OrderID");
        }
    }
}
