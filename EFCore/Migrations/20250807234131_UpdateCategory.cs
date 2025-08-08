using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "AIBlog_Category",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "IsDeleted",
                table: "AIBlog_Category",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AIBlog_Category");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AIBlog_Category",
                newName: "DisplayName");
        }
    }
}
