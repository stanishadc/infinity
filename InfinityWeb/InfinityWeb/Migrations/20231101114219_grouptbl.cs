using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityWeb.Migrations
{
    /// <inheritdoc />
    public partial class grouptbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Groups",
                type: "varchar(10)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Groups",
                type: "bit",
                nullable: true);
        }
    }
}
