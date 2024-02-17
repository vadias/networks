using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelsEx.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "from_user_id",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_from_user_id",
                table: "Messages",
                column: "from_user_id");

            migrationBuilder.AddForeignKey(
                name: "messages_from_user_id_fkey",
                table: "Messages",
                column: "from_user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "messages_from_user_id_fkey",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_from_user_id",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "from_user_id",
                table: "Messages");
        }
    }
}
