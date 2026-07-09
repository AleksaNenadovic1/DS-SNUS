using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IngestionService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consensus_values",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consensus_values", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "event_logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SensorId = table.Column<int>(type: "integer", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "processed_messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SensorId = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    BlockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Quality = table.Column<int>(type: "integer", nullable: false),
                    MinTemperature = table.Column<double>(type: "double precision", nullable: false),
                    MaxTemperature = table.Column<double>(type: "double precision", nullable: false),
                    Alarm1Limit = table.Column<double>(type: "double precision", nullable: false),
                    Alarm2Limit = table.Column<double>(type: "double precision", nullable: false),
                    Alarm3Limit = table.Column<double>(type: "double precision", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "measurements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SensorId = table.Column<int>(type: "integer", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlarmPriority = table.Column<int>(type: "integer", nullable: false),
                    Quality = table.Column<int>(type: "integer", nullable: false),
                    IsConsensus = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_measurements_sensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "sensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_measurements_SensorId",
                table: "measurements",
                column: "SensorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consensus_values");

            migrationBuilder.DropTable(
                name: "event_logs");

            migrationBuilder.DropTable(
                name: "measurements");

            migrationBuilder.DropTable(
                name: "processed_messages");

            migrationBuilder.DropTable(
                name: "sensors");
        }
    }
}
