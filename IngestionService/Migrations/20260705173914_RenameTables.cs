using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IngestionService.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurement_Sensor_SensorId",
                table: "Measurement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessedMessage",
                table: "ProcessedMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Measurement",
                table: "Measurement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventLog",
                table: "EventLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsensusValue",
                table: "ConsensusValue");

            migrationBuilder.RenameTable(
                name: "Sensor",
                newName: "sensors");

            migrationBuilder.RenameTable(
                name: "ProcessedMessage",
                newName: "processed_messages");

            migrationBuilder.RenameTable(
                name: "Measurement",
                newName: "measurements");

            migrationBuilder.RenameTable(
                name: "EventLog",
                newName: "event_logs");

            migrationBuilder.RenameTable(
                name: "ConsensusValue",
                newName: "consensus_values");

            migrationBuilder.RenameIndex(
                name: "IX_Measurement_SensorId",
                table: "measurements",
                newName: "IX_measurements_SensorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sensors",
                table: "sensors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_measurements",
                table: "measurements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_event_logs",
                table: "event_logs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_consensus_values",
                table: "consensus_values",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_measurements_sensors_SensorId",
                table: "measurements",
                column: "SensorId",
                principalTable: "sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_measurements_sensors_SensorId",
                table: "measurements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sensors",
                table: "sensors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_processed_messages",
                table: "processed_messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_measurements",
                table: "measurements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_event_logs",
                table: "event_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_consensus_values",
                table: "consensus_values");

            migrationBuilder.RenameTable(
                name: "sensors",
                newName: "Sensor");

            migrationBuilder.RenameTable(
                name: "processed_messages",
                newName: "ProcessedMessage");

            migrationBuilder.RenameTable(
                name: "measurements",
                newName: "Measurement");

            migrationBuilder.RenameTable(
                name: "event_logs",
                newName: "EventLog");

            migrationBuilder.RenameTable(
                name: "consensus_values",
                newName: "ConsensusValue");

            migrationBuilder.RenameIndex(
                name: "IX_measurements_SensorId",
                table: "Measurement",
                newName: "IX_Measurement_SensorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessedMessage",
                table: "ProcessedMessage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Measurement",
                table: "Measurement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventLog",
                table: "EventLog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsensusValue",
                table: "ConsensusValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurement_Sensor_SensorId",
                table: "Measurement",
                column: "SensorId",
                principalTable: "Sensor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
