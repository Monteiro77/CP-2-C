using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDigital.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_AGENCIA",
                columns: table => new
                {
                    ID_AGENCIA = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_AGENCIA = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NR_CODIGO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    DS_ENDERECO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_AGENCIA", x => x.ID_AGENCIA);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRODUTO",
                columns: table => new
                {
                    ID_PRODUTO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_PRODUTO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DS_PRODUTO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TP_PRODUTO = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    VL_MINIMO = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    VL_MAXIMO = table.Column<decimal>(type: "NUMBER(18,2)", nullable: true),
                    PC_TAXA_JUROS = table.Column<decimal>(type: "NUMBER(5,4)", nullable: true),
                    PC_TAXA_MDR = table.Column<decimal>(type: "NUMBER(5,4)", nullable: true),
                    NM_EMPRESA = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRODUTO", x => x.ID_PRODUTO);
                });

            migrationBuilder.CreateTable(
                name: "TB_CLIENTE",
                columns: table => new
                {
                    ID_CLIENTE = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_NOME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    DS_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ID_AGENCIA = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TP_CLIENTE = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    NR_CPF = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: true),
                    DT_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NR_CNPJ = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    DS_RAZAO_SOCIAL = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CLIENTE", x => x.ID_CLIENTE);
                    table.ForeignKey(
                        name: "FK_TB_CLIENTE_TB_AGENCIA_ID_AGENCIA",
                        column: x => x.ID_AGENCIA,
                        principalTable: "TB_AGENCIA",
                        principalColumn: "ID_AGENCIA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONTRATACAO",
                columns: table => new
                {
                    ID_CONTRATACAO = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLIENTE = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ID_PRODUTO = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TP_STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    VL_SOLICITADO = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    DT_SOLICITACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_PROCESSAMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DS_OBSERVACAO = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    NR_SCORE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONTRATACAO", x => x.ID_CONTRATACAO);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACAO_TB_CLIENTE_ID_CLIENTE",
                        column: x => x.ID_CLIENTE,
                        principalTable: "TB_CLIENTE",
                        principalColumn: "ID_CLIENTE",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACAO_TB_PRODUTO_ID_PRODUTO",
                        column: x => x.ID_PRODUTO,
                        principalTable: "TB_PRODUTO",
                        principalColumn: "ID_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TB_PRODUTO",
                columns: new[] { "ID_PRODUTO", "DS_PRODUTO", "NM_PRODUTO", "TP_PRODUTO", "PC_TAXA_JUROS", "VL_MAXIMO", "VL_MINIMO" },
                values: new object[] { 1L, "Empréstimo pessoal com análise de crédito automatizada", "Empréstimo Pessoal", "EMPRESTIMO", 0.0299m, 50000m, 1000m });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTE_ID_AGENCIA",
                table: "TB_CLIENTE",
                column: "ID_AGENCIA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTE_NR_CNPJ",
                table: "TB_CLIENTE",
                column: "NR_CNPJ",
                unique: true,
                filter: "\"NR_CNPJ\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTE_NR_CPF",
                table: "TB_CLIENTE",
                column: "NR_CPF",
                unique: true,
                filter: "\"NR_CPF\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACAO_ID_CLIENTE",
                table: "TB_CONTRATACAO",
                column: "ID_CLIENTE");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACAO_ID_PRODUTO",
                table: "TB_CONTRATACAO",
                column: "ID_PRODUTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CONTRATACAO");

            migrationBuilder.DropTable(
                name: "TB_CLIENTE");

            migrationBuilder.DropTable(
                name: "TB_PRODUTO");

            migrationBuilder.DropTable(
                name: "TB_AGENCIA");
        }
    }
}
