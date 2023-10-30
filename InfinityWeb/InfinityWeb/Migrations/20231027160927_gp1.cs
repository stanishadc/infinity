using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityWeb.Migrations
{
    /// <inheritdoc />
    public partial class gp1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupTypeName",
                table: "GroupTypes",
                type: "varchar(25)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupTypeName",
                table: "GroupTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)");
        }
    }
}
