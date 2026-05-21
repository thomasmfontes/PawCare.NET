using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawCareApi.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLINICA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME_CNPJ = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    BAIRRO = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    ATENDIMENTO_24H = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLINICA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MEDICO_ESPECIALISTA",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIALIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICO_ESPECIALISTA", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TRATAMENTO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    NOME_MEDICAMENTO = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    FREQUENCIA = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    DATA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_FINAL = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRATAMENTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TRATAMENTO_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TIPO = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    ID_PET = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_TUTOR = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    ID_MEDICO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_CLINICA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EVENTO_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "CLINICA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVENTO_MEDICO_ESPECIALISTA_ID_MEDICO",
                        column: x => x.ID_MEDICO,
                        principalTable: "MEDICO_ESPECIALISTA",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVENTO_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EVENTO_TUTOR_ID_TUTOR",
                        column: x => x.ID_TUTOR,
                        principalTable: "TUTOR",
                        principalColumn: "CPF",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HISTORICO_CLINICO",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_EVENTO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    DATA_EVENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_VENCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    OBSERVACOES_IA = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORICO_CLINICO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HISTORICO_CLINICO_EVENTO_ID_EVENTO",
                        column: x => x.ID_EVENTO,
                        principalTable: "EVENTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_ID_CLINICA",
                table: "EVENTO",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_ID_MEDICO",
                table: "EVENTO",
                column: "ID_MEDICO");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_ID_PET",
                table: "EVENTO",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_ID_TUTOR",
                table: "EVENTO",
                column: "ID_TUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICO_CLINICO_ID_EVENTO",
                table: "HISTORICO_CLINICO",
                column: "ID_EVENTO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TRATAMENTO_ID_PET",
                table: "TRATAMENTO",
                column: "ID_PET");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HISTORICO_CLINICO");

            migrationBuilder.DropTable(
                name: "TRATAMENTO");

            migrationBuilder.DropTable(
                name: "EVENTO");

            migrationBuilder.DropTable(
                name: "CLINICA");

            migrationBuilder.DropTable(
                name: "MEDICO_ESPECIALISTA");
        }
    }
}
