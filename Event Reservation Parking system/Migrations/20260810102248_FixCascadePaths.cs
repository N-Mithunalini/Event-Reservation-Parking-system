using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventParkingReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSeats_Events_EventId",
                table: "BookingSeats");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSeats_Seats_SeatId",
                table: "BookingSeats");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingReservations_Events_EventId",
                table: "ParkingReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingReservations_ParkingSlots_ParkingSlotId",
                table: "ParkingReservations");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingNumber",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "BookingNumber",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSeats_Events_EventId",
                table: "BookingSeats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSeats_Seats_SeatId",
                table: "BookingSeats",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingReservations_Events_EventId"
                onDelete: ReferentialAction.NoAction,
                table: "ParkingReservations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingReservations_ParkingSlots_ParkingSlotId",
                table: "ParkingReservations",
                column: "ParkingSlotId",
                principalTable: "ParkingSlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSeats_Events_EventId",
                table: "BookingSeats");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingSeats_Seats_SeatId",
                table: "BookingSeats");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingReservations_Events_EventId",
                table: "ParkingReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingReservations_ParkingSlots_ParkingSlotId",
                table: "ParkingReservations");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BookingNumber",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingNumber",
                table: "Bookings",
                column: "BookingNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Events_EventId",
                table: "Bookings",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSeats_Events_EventId",
                table: "BookingSeats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingSeats_Seats_SeatId",
                table: "BookingSeats",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingReservations_Events_EventId",
                table: "ParkingReservations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingReservations_ParkingSlots_ParkingSlotId",
                table: "ParkingReservations",
                column: "ParkingSlotId",
                principalTable: "ParkingSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
