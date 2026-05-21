using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawCareApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RACA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RACA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TUTOR",
                columns: table => new
                {
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    QTD_PETS = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUTOR", x => x.CPF);
                });

            migrationBuilder.CreateTable(
                name: "PET",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PESO = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    STATUS_LONGEVIDADE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RACA_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TUTOR_CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PET_RACA_RACA_ID",
                        column: x => x.RACA_ID,
                        principalTable: "RACA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PET_TUTOR_TUTOR_CPF",
                        column: x => x.TUTOR_CPF,
                        principalTable: "TUTOR",
                        principalColumn: "CPF",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PET_RACA_ID",
                table: "PET",
                column: "RACA_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PET_TUTOR_CPF",
                table: "PET",
                column: "TUTOR_CPF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "RACA");

            migrationBuilder.DropTable(
                name: "TUTOR");
        }
    }
}
