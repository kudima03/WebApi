using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksAPI.Infrastructure.BooksContextMigrations
{
    public partial class StoringImagesChanging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinaryPhoto",
                table: "BookCard");

            migrationBuilder.AddColumn<string>(
                name: "PictureFileName",
                table: "BookCard",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureFileName",
                table: "BookCard");

            migrationBuilder.AddColumn<byte[]>(
                name: "BinaryPhoto",
                table: "BookCard",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
