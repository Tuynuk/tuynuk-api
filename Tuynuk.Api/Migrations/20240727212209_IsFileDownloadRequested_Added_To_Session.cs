using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tuynuk.Api.Migrations
{
    /// <inheritdoc />
    public partial class IsFileDownloadRequested_Added_To_Session : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFileDownloadRequested",
                table: "Sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFileDownloadRequested",
                table: "Sessions");
        }
    }
}
