using Microsoft.EntityFrameworkCore.Migrations;

namespace YAHCMS.BlogService.Migrations
{
    public partial class BlogUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "blogs",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserID",
                table: "blogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
