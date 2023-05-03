using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pw_gruppo1");

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "pw_gruppo1",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    avg_heart_rate = table.Column<int>(type: "integer", nullable: false),
                    session_distance = table.Column<int>(type: "integer", nullable: false),
                    pool_laps = table.Column<short>(type: "smallint", nullable: false),
                    pool_lenght = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.session_id);
                });

            migrationBuilder.CreateTable(
                name: "smartwatches",
                schema: "pw_gruppo1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    heart_rate = table.Column<int>(type: "integer", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                });

            migrationBuilder.CreateIndex(
                name: "IX_sessions_session_id",
                schema: "pw_gruppo1",
                table: "sessions",
                column: "session_id",
                unique: true);

            migrationBuilder.Sql(
                "SELECT create_hypertable('pw_gruppo1.smartwatches','timestamp');");

            migrationBuilder.Sql(
                "CREATE INDEX ix_userid_time ON pw_gruppo1.smartwatches (user_id, timestamp DESC);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sessions",
                schema: "pw_gruppo1");

            migrationBuilder.DropTable(
                name: "smartwatches",
                schema: "pw_gruppo1");
        }
    }
}
