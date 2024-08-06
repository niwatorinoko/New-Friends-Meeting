using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MeetingDateTime",
                table: "TablePosts1111675",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "TablePlayers1111675",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TablePosts1111675_PlayerId",
                table: "TablePosts1111675",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TablePosts1111675_TablePlayers1111675_PlayerId",
                table: "TablePosts1111675",
                column: "PlayerId",
                principalTable: "TablePlayers1111675",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TablePosts1111675_TablePlayers1111675_PlayerId",
                table: "TablePosts1111675");

            migrationBuilder.DropIndex(
                name: "IX_TablePosts1111675_PlayerId",
                table: "TablePosts1111675");

            migrationBuilder.DropColumn(
                name: "MeetingDateTime",
                table: "TablePosts1111675");

            migrationBuilder.AlterColumn<int>(
                name: "Photo",
                table: "TablePlayers1111675",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
