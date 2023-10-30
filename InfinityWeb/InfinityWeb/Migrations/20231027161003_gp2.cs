using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityWeb.Migrations
{
    /// <inheritdoc />
    public partial class gp2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupTypeName",
                table: "GroupTypes",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(25)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupTypeName",
                table: "GroupTypes",
                type: "varchar(25)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");
        }
    }
}
