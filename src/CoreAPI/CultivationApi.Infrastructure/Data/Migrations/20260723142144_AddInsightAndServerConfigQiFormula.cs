using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CultivationApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInsightAndServerConfigQiFormula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QiPerMessage",
                table: "ServerConfigs",
                newName: "MinQiPerMessage");

            migrationBuilder.AddColumn<double>(
                name: "InsightMultiplier",
                table: "ServerConfigs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MaxQiPerMessage",
                table: "ServerConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsightMultiplier",
                table: "ServerConfigs");

            migrationBuilder.DropColumn(
                name: "MaxQiPerMessage",
                table: "ServerConfigs");

            migrationBuilder.RenameColumn(
                name: "MinQiPerMessage",
                table: "ServerConfigs",
                newName: "QiPerMessage");
        }
    }
}
