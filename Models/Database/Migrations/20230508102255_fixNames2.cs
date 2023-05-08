using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class fixNames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pool_lenght",
                schema: "pw_gruppo1",
                table: "sessions",
                newName: "pool_length");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pool_length",
                schema: "pw_gruppo1",
                table: "sessions",
                newName: "pool_lenght");
        }
    }
}
