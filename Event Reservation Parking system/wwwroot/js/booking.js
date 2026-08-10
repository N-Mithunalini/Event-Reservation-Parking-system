let selectedSeats = [];
let selectedParking = null;
let currentEvent = null;
let eventId = 0;


// ======================================================
// LOAD EVENT + SEATS + PARKING
// ======================================================

async function loadBooking() {

    eventId = Number(
        new URLSearchParams(location.search).get("id")
    );

    if (!eventId) {
        return;
    }

    try {

        const [ev, seats, slots] = await Promise.all([
            api(`/events/${eventId}`),
            api(`/events/${eventId}/seats`),
            api(`/events/${eventId}/parking-slots`)
        ]);

        currentEvent = ev;


        // Event Details
        eventName.textContent = ev.name;

        eventInfo.textContent = `
            ${ev.venue?.name || ""}
            •
            ${new Date(ev.eventDate).toLocaleDateString()}
            •
            Ticket Rs. ${ev.ticketPrice}
        `;


        // ======================================================
        // SEAT MAP
        // ======================================================

        seatGrid.innerHTML = seats.length
            ? seats.map(s => `
                <button
                    class="seat ${s.status}"
                    data-id="${s.id}"
                    data-number="${s.seatNumber}"
                    ${s.status !== "Available" ? "disabled" : ""}>

                    ${s.seatNumber}

                </button>
            `).join("")
            : `
                <div class="alert alert-warning">
                    No seat map yet.
                </div>
            `;


        // ======================================================
        // PARKING MAP
        // ======================================================

        parkingGrid.innerHTML = slots.length
            ? slots.map(s => `
                <button
                    class="slot ${s.status}"
                    data-id="${s.id}"
                    data-fee="${s.fee}"
                    ${s.status !== "Available" ? "disabled" : ""}>

                    ${s.slotNumber}

                    <br>

                    <small>
                        Rs. ${s.fee}
                    </small>

                </button>
            `).join("")
            : `
                <div class="alert alert-info">
                    Parking not available.
                </div>
            `;


        // ======================================================
        // SEAT SELECTION
        // ======================================================

        seatGrid.onclick = (e) => {

            const button =
                e.target.closest(".seat");

            if (!button) {
                return;
            }

            const id =
                Number(button.dataset.id);

            button.classList.toggle(
                "selected"
            );

            if (
                button.classList.contains("selected")
            ) {
                selectedSeats = [
                    ...selectedSeats,
                    id
                ];
            }
            else {
                selectedSeats =
                    selectedSeats.filter(
                        x => x !== id
                    );
            }

            summary();
        };


        // ======================================================
        // PARKING SELECTION
        // ======================================================

        parkingGrid.onclick = (e) => {

            const button =
                e.target.closest(".slot");

            if (!button) {
                return;
            }

            const id =
                Number(button.dataset.id);


            // Remove previous selected parking slot
            parkingGrid
                .querySelectorAll(".slot")
                .forEach(x =>
                    x.classList.remove("selected")
                );


            // Clicking selected parking again removes selection
            if (selectedParking === id) {

                selectedParking = null;

            }
            else {

                selectedParking = id;

                button.classList.add(
                    "selected"
                );
            }

            summary();
        };


        // Booking Button
        bookBtn.onclick =
            createBooking;


        summary();

    }
    catch (e) {

        bookingMsg.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;

    }
}


// ======================================================
// BOOKING SUMMARY
// ======================================================

function summary() {

    const names = [
        ...document.querySelectorAll(
            ".seat.selected"
        )
    ].map(
        x => x.dataset.number
    );


    selectedSeatsText.textContent =
        names.join(", ") || "None";


    const parking =
        document.querySelector(
            ".slot.selected"
        );


    const seatTotal =
        selectedSeats.length *
        (currentEvent?.ticketPrice || 0);


    const parkingFee =
        parking
            ? Number(parking.dataset.fee)
            : 0;


    const total =
        seatTotal + parkingFee;


    totalText.textContent =
        total.toFixed(2);
}


// ======================================================
// CREATE BOOKING
// ======================================================

async function createBooking() {

    const user =
        currentUser();


    // Customer must login
    if (
        !user ||
        ((user.role || user.Role) !== "Customer")
    ) {
        location.href =
            "login.html";

        return;
    }


    // At least one seat required
    if (!selectedSeats.length) {

        alert(
            "Select at least one seat."
        );

        return;
    }


    // Confirmation
    if (
        !confirm(
            "Confirm this booking?"
        )
    ) {
        return;
    }


    try {

        const booking =
            await api(
                "/bookings",
                {
                    method: "POST",

                    body: JSON.stringify({
                        customerId: user.id,
                        eventId: eventId,
                        seatIds: selectedSeats,
                        parkingSlotId: selectedParking
                    })
                }
            );


        alert(
            `Booking created: ${booking.bookingNumber}`
        );


        location.href =
            "my-bookings.html";

    }
    catch (e) {

        alert(
            e.message
        );

    }
}


// ======================================================
// PAGE LOAD
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    loadBooking
);