using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfinityWeb.Migrations
{
    /// <inheritdoc />
    public partial class grp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Groups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Groups",
                type: "varchar(10)",
                nullable: true);
        }
    }
}
