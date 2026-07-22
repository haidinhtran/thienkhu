using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CultivationApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServerConfigChatToEarn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatToEarnChannels",
                table: "ServerConfigs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MessageCooldownSeconds",
                table: "ServerConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QiPerMessage",
                table: "ServerConfigs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatToEarnChannels",
                table: "ServerConfigs");

            migrationBuilder.DropColumn(
                name: "MessageCooldownSeconds",
                table: "ServerConfigs");

            migrationBuilder.DropColumn(
                name: "QiPerMessage",
                table: "ServerConfigs");
        }
    }
}
