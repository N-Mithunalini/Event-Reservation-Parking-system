# Event & Parking Reservation System - Full Working Build

Open `EventParkingReservationSystem.sln` in Visual Studio 2022 and press F5. The app automatically creates a fresh LocalDB database named `EventParkingReservationDbFull`, applies the included migration, and seeds one demo event with seats and parking.

Customer: Register -> verify using the generated token -> login.
Admin demo login: `admin@eventparking.com` / `Admin@123`.
Swagger: `/swagger`.

If LocalDB is not installed, change `DefaultConnection` in `appsettings.json` to your SQL Server instance.
