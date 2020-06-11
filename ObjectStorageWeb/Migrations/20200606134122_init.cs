using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ObjectStorageWeb.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "model-classes",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_model-classes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "model-properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ClassName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_model-properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_model-properties_model-classes_ClassName",
                        column: x => x.ClassName,
                        principalTable: "model-classes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_model-properties_ClassName",
                table: "model-properties",
                column: "ClassName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "model-properties");

            migrationBuilder.DropTable(
                name: "model-classes");
        }
    }
}
