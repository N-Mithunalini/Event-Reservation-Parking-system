let venues = [];
let categories = [];
let eventList = [];


// ======================================================
// ADMIN CHECK
// ======================================================

function adminCheck() {
    const user = currentUser();

    if (!user || ((user.role || user.Role) !== "Admin")) {
        location.href = "login.html";
        return false;
    }

    return true;
}


// ======================================================
// SHOW ADMIN SECTION
// ======================================================

function showSection(id) {
    document
        .querySelectorAll(".admin-section")
        .forEach(section => {
            section.classList.remove("active");
        });

    const target = document.querySelector("#" + id);

    if (target) {
        target.classList.add("active");
    }
}


// ======================================================
// LOAD ADMIN PAGE
// ======================================================

async function loadAdmin() {
    if (!adminCheck()) {
        return;
    }

    try {
        // These do not depend on eventList
        await Promise.all([
            loadStats(),
            loadVenues(),
            loadCategories(),
            loadCustomers()
        ]);

        // IMPORTANT:
        // Events must load before bookings.
        await loadEventsAdmin();

        // Now eventList contains events.
        await loadBookingsAdmin();
    }
    catch (e) {
        console.error("Admin loading error:", e);
        alert(e.message);
    }
}


// ======================================================
// DASHBOARD STATISTICS
// ======================================================

async function loadStats() {
    try {
        const data = await api("/dashboard/admin");

        adminStats.innerHTML = Object.entries(data)
            .map(([key, value]) => `
                <div class="col-md-4">

                    <div class="card p-3 stat">

                        <small>
                            ${key}
                        </small>

                        <h2>
                            ${value}
                        </h2>

                    </div>

                </div>
            `)
            .join("");
    }
    catch (e) {
        adminStats.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;
    }
}


// ======================================================
// VENUE MANAGEMENT
// ======================================================

async function loadVenues() {
    try {
        venues = await api("/venues");

        venueRows.innerHTML = venues.length
            ? venues.map(venue => `
                <tr>

                    <td>
                        ${venue.name}
                    </td>

                    <td>
                        ${venue.address}
                    </td>

                    <td>
                        ${venue.capacity}
                    </td>

                    <td>

                        <button
                            class="btn btn-sm btn-outline-primary"
                            onclick="editVenue(${venue.id})">

                            Edit

                        </button>

                        <button
                            class="btn btn-sm btn-outline-danger"
                            onclick="delVenue(${venue.id})">

                            Delete

                        </button>

                    </td>

                </tr>
            `).join("")
            : `
                <tr>
                    <td colspan="4"
                        class="text-center text-muted">

                        No venues found.

                    </td>
                </tr>
            `;

        fillSelects();
    }
    catch (e) {
        console.error("Venue load error:", e);
    }
}


async function addVenue() {
    try {
        const name = vName.value.trim();
        const address = vAddress.value.trim();
        const capacity = Number(vCapacity.value);

        if (!name || !address || capacity <= 0) {
            alert("Please enter valid venue details.");
            return;
        }

        await api("/venues", {
            method: "POST",

            body: JSON.stringify({
                name: name,
                address: address,
                capacity: capacity
            })
        });

        vName.value = "";
        vAddress.value = "";
        vCapacity.value = "";

        await loadVenues();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


async function editVenue(id) {
    const venue = venues.find(x => x.id === id);

    if (!venue) {
        return;
    }

    const name = prompt(
        "Venue name",
        venue.name
    );

    if (name === null) {
        return;
    }

    const address = prompt(
        "Address",
        venue.address
    );

    if (address === null) {
        return;
    }

    const capacityInput = prompt(
        "Capacity",
        venue.capacity
    );

    if (capacityInput === null) {
        return;
    }

    const capacity = Number(capacityInput);

    if (!name.trim() || !address.trim() || capacity <= 0) {
        alert("Invalid venue details.");
        return;
    }

    try {
        await api(`/venues/${id}`, {
            method: "PUT",

            body: JSON.stringify({
                name: name.trim(),
                address: address.trim(),
                capacity: capacity
            })
        });

        await loadVenues();
    }
    catch (e) {
        alert(e.message);
    }
}


async function delVenue(id) {
    if (!confirm("Delete venue?")) {
        return;
    }

    try {
        await api(`/venues/${id}`, {
            method: "DELETE"
        });

        await loadVenues();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// CATEGORY MANAGEMENT
// ======================================================

async function loadCategories() {
    try {
        categories = await api("/categories");

        categoryRows.innerHTML = categories.length
            ? categories.map(category => `
                <tr>

                    <td>
                        ${category.name}
                    </td>

                    <td>

                        <button
                            class="btn btn-sm btn-outline-primary"
                            onclick="editCategory(${category.id})">

                            Edit

                        </button>

                        <button
                            class="btn btn-sm btn-outline-danger"
                            onclick="delCategory(${category.id})">

                            Delete

                        </button>

                    </td>

                </tr>
            `).join("")
            : `
                <tr>
                    <td colspan="2"
                        class="text-center text-muted">

                        No categories found.

                    </td>
                </tr>
            `;

        fillSelects();
    }
    catch (e) {
        console.error("Category load error:", e);
    }
}


async function addCategory() {
    try {
        const name = cName.value.trim();

        if (!name) {
            alert("Enter category name.");
            return;
        }

        await api("/categories", {
            method: "POST",

            body: JSON.stringify({
                name: name
            })
        });

        cName.value = "";

        await loadCategories();
    }
    catch (e) {
        alert(e.message);
    }
}


async function editCategory(id) {
    const category = categories.find(
        x => x.id === id
    );

    if (!category) {
        return;
    }

    const name = prompt(
        "Category name",
        category.name
    );

    if (name === null) {
        return;
    }

    if (!name.trim()) {
        alert("Category name required.");
        return;
    }

    try {
        await api(`/categories/${id}`, {
            method: "PUT",

            body: JSON.stringify({
                name: name.trim()
            })
        });

        await loadCategories();
    }
    catch (e) {
        alert(e.message);
    }
}


async function delCategory(id) {
    if (!confirm("Delete category?")) {
        return;
    }

    try {
        await api(`/categories/${id}`, {
            method: "DELETE"
        });

        await loadCategories();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// DROPDOWN DATA
// ======================================================

function fillSelects() {
    if (window.eVenue) {
        eVenue.innerHTML = venues.length
            ? venues.map(venue => `
                <option value="${venue.id}">
                    ${venue.name}
                </option>
            `).join("")
            : `
                <option value="">
                    No venues
                </option>
            `;
    }


    if (window.eCategory) {
        eCategory.innerHTML = categories.length
            ? categories.map(category => `
                <option value="${category.id}">
                    ${category.name}
                </option>
            `).join("")
            : `
                <option value="">
                    No categories
                </option>
            `;
    }


    if (window.layoutEvent) {
        layoutEvent.innerHTML = eventList.length
            ? eventList.map(event => `
                <option value="${event.id}">
                    ${event.name}
                </option>
            `).join("")
            : `
                <option value="">
                    No events
                </option>
            `;
    }
}


// ======================================================
// EVENT MANAGEMENT
// ======================================================

async function loadEventsAdmin() {
    try {
        eventList = await api("/events");

        eventRows.innerHTML = eventList.length
            ? eventList.map(event => `
                <tr>

                    <td>
                        ${event.name}
                    </td>

                    <td>
                        ${event.venue?.name || ""}
                    </td>

                    <td>
                        ${new Date(
                event.eventDate
            ).toLocaleDateString()}
                    </td>

                    <td>
                        ${event.capacity}
                    </td>

                    <td>

                        <button
                            class="btn btn-sm btn-outline-primary"
                            onclick="editEvent(${event.id})">

                            Edit

                        </button>

                        <button
                            class="btn btn-sm btn-outline-danger"
                            onclick="delEvent(${event.id})">

                            Delete

                        </button>

                    </td>

                </tr>
            `).join("")
            : `
                <tr>
                    <td colspan="5"
                        class="text-center text-muted">

                        No events found.

                    </td>
                </tr>
            `;

        fillSelects();
        if (window.layoutEvent && layoutEvent.value) {
            await loadLayoutDetails();
        }
    }
    catch (e) {
        console.error("Event load error:", e);
        throw e;
    }
}


// ======================================================
// EVENT PAYLOAD
// ======================================================

function eventPayload() {
    return {
        name: eName.value.trim(),

        venueId: Number(
            eVenue.value
        ),

        categoryId: Number(
            eCategory.value
        ),

        eventDate:
            eDate.value,

        startTime:
            eStart.value.length === 5
                ? eStart.value + ":00"
                : eStart.value,

        endTime:
            eEnd.value.length === 5
                ? eEnd.value + ":00"
                : eEnd.value,

        ticketPrice: Number(
            eTicket.value
        ),

        parkingFee: Number(
            eParking.value
        ),

        capacity: Number(
            eCapacity.value
        )
    };
}


// ======================================================
// ADD EVENT
// ======================================================

async function addEvent() {
    const payload = eventPayload();

    if (
        !payload.name ||
        !payload.venueId ||
        !payload.categoryId ||
        !payload.eventDate ||
        !payload.startTime ||
        !payload.endTime ||
        payload.capacity <= 0
    ) {
        alert("Please fill all event fields.");
        return;
    }

    try {
        await api("/events", {
            method: "POST",
            body: JSON.stringify(payload)
        });

        clearEventForm();

        await loadEventsAdmin();
        await loadBookingsAdmin();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// CLEAR EVENT FORM
// ======================================================

function clearEventForm() {
    eName.value = "";
    eDate.value = "";
    eStart.value = "";
    eEnd.value = "";
    eTicket.value = "";
    eParking.value = "";
    eCapacity.value = "";

    eventSaveBtn.textContent =
        "Add Event";

    eventSaveBtn.onclick =
        addEvent;
}


// ======================================================
// EDIT EVENT
// ======================================================

async function editEvent(id) {
    const event = eventList.find(
        x => x.id === id
    );

    if (!event) {
        return;
    }

    eName.value =
        event.name;

    eVenue.value =
        event.venueId;

    eCategory.value =
        event.categoryId;

    eDate.value =
        (event.eventDate || "")
            .slice(0, 10);

    eStart.value =
        (event.startTime || "")
            .slice(0, 5);

    eEnd.value =
        (event.endTime || "")
            .slice(0, 5);

    eTicket.value =
        event.ticketPrice;

    eParking.value =
        event.parkingFee;

    eCapacity.value =
        event.capacity;


    eventSaveBtn.textContent =
        "Update Event";


    eventSaveBtn.onclick =
        async () => {

            try {
                await api(`/events/${id}`, {
                    method: "PUT",
                    body: JSON.stringify(
                        eventPayload()
                    )
                });

                clearEventForm();

                await loadEventsAdmin();
                await loadBookingsAdmin();
                await loadStats();
            }
            catch (e) {
                alert(e.message);
            }
        };
}


// ======================================================
// DELETE EVENT
// ======================================================

async function delEvent(id) {
    if (!confirm("Delete event?")) {
        return;
    }

    try {
        await api(`/events/${id}`, {
            method: "DELETE"
        });

        await loadEventsAdmin();
        await loadBookingsAdmin();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// SEAT MAP GENERATOR
// ======================================================

async function generateSeats() {
    if (!layoutEvent.value) {
        alert("Please select an event.");
        return;
    }

    const rows =
        Number(seatRows.value);

    const columns =
        Number(seatCols.value);

    if (
        rows <= 0 ||
        columns <= 0
    ) {
        alert(
            "Enter valid rows and columns."
        );

        return;
    }

    try {
        await api(
            `/events/${layoutEvent.value}/seats`,
            {
                method: "POST",

                body: JSON.stringify({
                    rows: rows,
                    columns: columns
                })
            }
        );

        alert(
            "Seat map generated successfully."
        );

        await loadStats();
        await loadLayoutDetails();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// PARKING LAYOUT GENERATOR
// ======================================================

async function generateParking() {
    if (!layoutEvent.value) {
        alert("Please select an event.");
        return;
    }

    const count =
        Number(slotCount.value);

    const fee =
        Number(slotFee.value);

    if (count <= 0) {
        alert(
            "Enter valid parking slot count."
        );

        return;
    }

    try {
        await api(
            `/events/${layoutEvent.value}/parking-slots`,
            {
                method: "POST",

                body: JSON.stringify({
                    slotCount: count,
                    zone: slotZone.value.trim(),
                    fee: fee
                })
            }
        );

        alert(
            "Parking layout generated successfully."
        );

        await loadStats();
        await loadLayoutDetails();
    }
    catch (e) {
        alert(e.message);
    }
}



// ======================================================
// EXISTING SEAT / PARKING MANAGEMENT
// ======================================================

async function loadLayoutDetails() {
    if (!window.layoutEvent || !layoutEvent.value) {
        if (window.seatAdminRows) seatAdminRows.innerHTML = "";
        if (window.parkingAdminRows) parkingAdminRows.innerHTML = "";
        return;
    }

    try {
        const eventId = Number(layoutEvent.value);
        const [seats, slots] = await Promise.all([
            api(`/events/${eventId}/seats`),
            api(`/events/${eventId}/parking-slots`)
        ]);

        if (window.seatAdminRows) {
            seatAdminRows.innerHTML = seats.length
                ? seats.map(seat => `
                    <tr>
                        <td>${seat.seatNumber}</td>
                        <td>${seat.status}</td>
                        <td>${seat.seatType || "-"}</td>
                        <td>
                            <button class="btn btn-sm btn-outline-primary" onclick="editSeat(${eventId}, ${seat.id})">Edit</button>
                            <button class="btn btn-sm btn-outline-danger" onclick="deleteSeat(${eventId}, ${seat.id})">Delete</button>
                        </td>
                    </tr>
                `).join("")
                : `<tr><td colspan="4" class="text-center text-muted">No seats generated.</td></tr>`;
        }

        if (window.parkingAdminRows) {
            parkingAdminRows.innerHTML = slots.length
                ? slots.map(slot => `
                    <tr>
                        <td>${slot.slotNumber}</td>
                        <td>${slot.zone || "-"}</td>
                        <td>Rs. ${Number(slot.fee).toFixed(2)}</td>
                        <td>${slot.status}</td>
                        <td>
                            <button class="btn btn-sm btn-outline-primary" onclick="editParkingSlot(${eventId}, ${slot.id})">Edit</button>
                            <button class="btn btn-sm btn-outline-danger" onclick="deleteParkingSlot(${eventId}, ${slot.id})">Delete</button>
                        </td>
                    </tr>
                `).join("")
                : `<tr><td colspan="5" class="text-center text-muted">No parking slots generated.</td></tr>`;
        }
    }
    catch (e) {
        console.error("Layout load error:", e);
    }
}

async function editSeat(eventId, seatId) {
    const seats = await api(`/events/${eventId}/seats`);
    const seat = seats.find(x => x.id === seatId);
    if (!seat) return;

    const seatNumber = prompt("Seat number", seat.seatNumber);
    if (seatNumber === null) return;
    const seatType = prompt("Seat type (optional)", seat.seatType || "");
    if (seatType === null) return;
    const status = prompt("Status: Available / Held / Booked", seat.status);
    if (status === null) return;

    try {
        await api(`/events/${eventId}/seats/${seatId}`, {
            method: "PUT",
            body: JSON.stringify({ seatNumber, seatType, status })
        });
        await loadLayoutDetails();
    }
    catch (e) { alert(e.message); }
}

async function deleteSeat(eventId, seatId) {
    if (!confirm("Delete this seat?")) return;
    try {
        await api(`/events/${eventId}/seats/${seatId}`, { method: "DELETE" });
        await loadLayoutDetails();
        await loadStats();
    }
    catch (e) { alert(e.message); }
}

async function editParkingSlot(eventId, slotId) {
    const slots = await api(`/events/${eventId}/parking-slots`);
    const slot = slots.find(x => x.id === slotId);
    if (!slot) return;

    const slotNumber = prompt("Slot number", slot.slotNumber);
    if (slotNumber === null) return;
    const zone = prompt("Zone", slot.zone || "");
    if (zone === null) return;
    const feeInput = prompt("Fee", slot.fee);
    if (feeInput === null) return;
    const status = prompt("Status: Available / Held / Reserved", slot.status);
    if (status === null) return;

    try {
        await api(`/events/${eventId}/parking-slots/${slotId}`, {
            method: "PUT",
            body: JSON.stringify({ slotNumber, zone, fee: Number(feeInput), status })
        });
        await loadLayoutDetails();
    }
    catch (e) { alert(e.message); }
}

async function deleteParkingSlot(eventId, slotId) {
    if (!confirm("Delete this parking slot?")) return;
    try {
        await api(`/events/${eventId}/parking-slots/${slotId}`, { method: "DELETE" });
        await loadLayoutDetails();
        await loadStats();
    }
    catch (e) { alert(e.message); }
}

// ======================================================
// CUSTOMER MANAGEMENT
// ======================================================

async function loadCustomers(searchTerm = "") {
    try {
        const query = searchTerm?.trim() ? `?search=${encodeURIComponent(searchTerm.trim())}` : "";
        const rows = await api(`/customers${query}`);

        customerRows.innerHTML = rows.length
            ? rows.map(customer => `
                <tr>

                    <td>
                        ${customer.name}
                    </td>

                    <td>
                        ${customer.email}
                    </td>

                    <td>
                        ${customer.status}
                    </td>

                    <td>
                        ${customer.emailVerified
                    ? "Yes"
                    : "No"
                }
                    </td>

                    <td>

                        ${customer.status === "Active"
                    ? `
                                    <button
                                        class="btn btn-sm btn-warning"
                                        onclick="deactivate(${customer.id})">

                                        Deactivate

                                    </button>
                                  `
                    : `
                                    <button
                                        class="btn btn-sm btn-success"
                                        onclick="reactivate(${customer.id})">

                                        Reactivate

                                    </button>
                                  `
                }

                    </td>

                </tr>
            `).join("")
            : `
                <tr>
                    <td colspan="5"
                        class="text-center text-muted">

                        No customers found.

                    </td>
                </tr>
            `;
    }
    catch (e) {
        console.error(
            "Customer load error:",
            e
        );
    }
}


// ======================================================
// DEACTIVATE CUSTOMER
// ======================================================

async function deactivate(id) {
    if (
        !confirm(
            "Deactivate this customer?"
        )
    ) {
        return;
    }

    try {
        await api(
            `/customers/${id}`,
            {
                method: "DELETE"
            }
        );

        await loadCustomers();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// REACTIVATE CUSTOMER
// ======================================================

async function reactivate(id) {
    try {
        await api(
            `/customers/${id}/reactivate`,
            {
                method: "POST"
            }
        );

        await loadCustomers();
        await loadStats();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// ADMIN BOOKING MANAGEMENT
// ======================================================

async function loadBookingsAdmin() {
    try {

        // Ensure events have loaded
        if (
            !eventList ||
            eventList.length === 0
        ) {
            eventList =
                await api("/events");
        }


        // No events means no bookings
        if (eventList.length === 0) {

            bookingRows.innerHTML = `
                <tr>

                    <td colspan="4"
                        class="text-center text-muted">

                        No bookings found.

                    </td>

                </tr>
            `;

            return;
        }


        // Fetch bookings for every event
        const bookingRequests =
            eventList.map(event =>
                api(
                    `/bookings?eventId=${event.id}`
                )
            );


        const bookingGroups =
            await Promise.all(
                bookingRequests
            );


        const allBookings =
            bookingGroups.flat();


        // No booking records
        if (allBookings.length === 0) {

            bookingRows.innerHTML = `
                <tr>

                    <td colspan="4"
                        class="text-center text-muted">

                        No bookings found.

                    </td>

                </tr>
            `;

            return;
        }


        // Display booking table
        bookingRows.innerHTML =
            allBookings
                .map(booking => `

                    <tr>

                        <td>

                            <strong>
                                ${booking.bookingNumber}
                            </strong>

                        </td>


                        <td>
                            ${booking.customer?.name
                    || "Unknown"
                    }
                        </td>


                        <td>

                            <span class="badge ${getBookingStatusClass(booking.status)}">

                                ${booking.status}

                            </span>

                        </td>


                        <td>
                            ${new Date(
                        booking.createdAt
                    ).toLocaleString()
                    }
                        </td>

                    </tr>

                `)
                .join("");
    }
    catch (e) {
        console.error(
            "Booking load error:",
            e
        );

        bookingRows.innerHTML = `
            <tr>

                <td colspan="4">

                    <div class="alert alert-danger mb-0">

                        ${e.message}

                    </div>

                </td>

            </tr>
        `;
    }
}


// ======================================================
// BOOKING STATUS BADGE
// ======================================================

function getBookingStatusClass(status) {
    switch (
    (status || "").toLowerCase()
    ) {

        case "confirmed":
            return "text-bg-success";

        case "pending":
            return "text-bg-warning";

        case "cancelled":
            return "text-bg-danger";

        case "expired":
            return "text-bg-secondary";

        default:
            return "text-bg-primary";
    }
}


// ======================================================
// PAGE LOAD
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    loadAdmin
);