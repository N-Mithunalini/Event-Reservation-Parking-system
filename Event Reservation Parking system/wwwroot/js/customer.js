// ======================================================
// CUSTOMER AUTH CHECK
// ======================================================

function getCustomer() {
    const user = currentUser();

    if (!user) {
        location.href = "login.html";
        return null;
    }

    const role = user.role || user.Role;

    if (role !== "Customer") {
        // Admin customer-only page open பண்ணினா
        if (role === "Admin") {
            location.href = "admin.html";
        }
        else {
            location.href = "login.html";
        }

        return null;
    }

    return user;
}


// ======================================================
// CUSTOMER DASHBOARD
// ======================================================

async function loadDashboard() {
    const user = getCustomer();

    if (!user) {
        return;
    }

    const customerNameElement =
        document.querySelector("#customerName");

    const statsElement =
        document.querySelector("#stats");

    if (customerNameElement) {
        customerNameElement.textContent =
            user.name || user.Name || "Customer";
    }

    if (!statsElement) {
        return;
    }

    try {
        const data = await api(
            `/dashboard/customer/${user.id}`
        );

        statsElement.innerHTML =
            Object.entries(data)
                .map(([key, value]) => `
                    <div class="col-md-3">

                        <div class="card p-3 stat">

                            <small>
                                ${formatLabel(key)}
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
        statsElement.innerHTML = `
            <div class="col-12">

                <div class="alert alert-danger">
                    ${e.message}
                </div>

            </div>
        `;
    }
}


// ======================================================
// FORMAT DASHBOARD LABEL
// ======================================================

function formatLabel(text) {
    return text
        .replace(/([A-Z])/g, " $1")
        .replace(/^./, char => char.toUpperCase())
        .trim();
}


// ======================================================
// CUSTOMER BOOKINGS
// ======================================================

async function loadBookings() {
    const user = getCustomer();

    if (!user) {
        return;
    }

    const bookingsElement =
        document.querySelector("#bookings");

    if (!bookingsElement) {
        return;
    }

    try {
        const rows = await api(
            `/bookings/customer/${user.id}`
        );

        if (!rows.length) {
            bookingsElement.innerHTML = `
                <div class="alert alert-info">
                    No bookings yet.
                </div>
            `;

            return;
        }

        bookingsElement.innerHTML =
            rows.map(booking => {

                const seatNumbers =
                    booking.bookingSeats
                        ?.map(x => x.seat?.seatNumber)
                        .filter(Boolean)
                        .join(", ")
                    || "None";


                const parkingNumber =
                    booking.parkingReservation
                        ?.parkingSlot
                        ?.slotNumber
                    || "None";


                return `
                    <div class="card p-4 mb-3">

                        <div class="d-flex justify-content-between">

                            <div>

                                <h5>
                                    ${booking.bookingNumber}
                                </h5>

                                <div class="mb-1">
                                    <strong>Event:</strong>
                                    ${booking.event?.name || ""}
                                </div>

                                <div class="mb-1">
                                    <strong>Seats:</strong>
                                    ${seatNumbers}
                                </div>

                                <div class="mb-1">
                                    <strong>Parking:</strong>
                                    ${parkingNumber}
                                </div>

                                <div>
                                    <strong>Created:</strong>
                                    ${booking.createdAt
                        ? new Date(
                            booking.createdAt
                        ).toLocaleString()
                        : ""
                    }
                                </div>

                            </div>


                            <span class="badge ${getStatusClass(booking.status)} align-self-start">
                                ${booking.status}
                            </span>

                        </div>


                        <div class="mt-3">

                            ${booking.status === "Pending"
                        ? `
                                        <button
                                            class="btn btn-success btn-sm"
                                            onclick="pay(${booking.id})">

                                            Pay

                                        </button>

                                        <button
                                            class="btn btn-outline-danger btn-sm"
                                            onclick="cancelBooking(${booking.id})">

                                            Cancel

                                        </button>
                                      `
                        : ""
                    }


                            ${booking.payment
                        ? `
                                        <a
                                            class="btn btn-outline-primary btn-sm"
                                            href="receipt.html?id=${booking.payment.id}">

                                            Receipt

                                        </a>
                                      `
                        : ""
                    }

                        </div>

                    </div>
                `;
            })
                .join("");
    }
    catch (e) {
        bookingsElement.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;
    }
}


// ======================================================
// BOOKING STATUS CLASS
// ======================================================

function getStatusClass(status) {
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
// PAYMENT
// ======================================================
async function pay(id) {

    if (!confirm("Do you want to complete this payment?")) {
        return;
    }

    try {

        const result = await api(
            `/bookings/${id}/payment`,
            {
                method: "POST"
            }
        );

        console.log(
            "Payment response:",
            result
        );

        alert(
            "Payment completed successfully."
        );

        // Refresh bookings
        await loadBookings();

    }
    catch (e) {

        console.error(
            "Payment Error:",
            e
        );

        alert(
            "Payment failed: " + e.message
        );
    }
}

// ======================================================
// CANCEL BOOKING
// ======================================================

async function cancelBooking(id) {
    if (!confirm("Cancel this booking?")) {
        return;
    }

    try {
        await api(
            `/bookings/${id}`,
            {
                method: "DELETE"
            }
        );

        alert("Booking cancelled.");

        await loadBookings();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// NOTIFICATIONS
// ======================================================

async function loadNotifications() {
    const user = getCustomer();

    if (!user) {
        return;
    }

    const notificationsElement =
        document.querySelector("#notifications");

    if (!notificationsElement) {
        return;
    }

    try {
        const rows = await api(
            `/notifications/customer/${user.id}`
        );

        if (!rows.length) {
            notificationsElement.innerHTML = `
                <div class="alert alert-info">
                    No notifications.
                </div>
            `;

            return;
        }

        notificationsElement.innerHTML =
            rows.map(notification => `

                <div class="card p-3 mb-3 ${notification.isRead
                    ? "opacity-75"
                    : ""
                }">

                    <div class="d-flex justify-content-between align-items-start">

                        <div>

                            <div class="fw-semibold">
                                ${notification.message}
                            </div>

                            <small class="text-muted">
                                ${new Date(
                    notification.createdAt
                ).toLocaleString()
                }
                            </small>

                        </div>


                        ${notification.isRead
                    ? `
                                    <span class="badge text-bg-secondary">
                                        Read
                                    </span>
                                  `
                    : `
                                    <span class="badge text-bg-primary">
                                        New
                                    </span>
                                  `
                }

                    </div>


                    ${!notification.isRead
                    ? `
                                <div class="mt-3">

                                    <button
                                        class="btn btn-sm btn-outline-primary"
                                        onclick="markRead(${notification.id})">

                                        Mark as Read

                                    </button>

                                </div>
                              `
                    : ""
                }

                </div>

            `)
                .join("");
    }
    catch (e) {
        notificationsElement.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;
    }
}


// ======================================================
// MARK NOTIFICATION AS READ
// ======================================================

async function markRead(id) {
    try {
        await api(
            `/notifications/${id}/read`,
            {
                method: "PUT"
            }
        );

        await loadNotifications();
    }
    catch (e) {
        alert(e.message);
    }
}


// ======================================================
// PAYMENT HISTORY
// ======================================================

async function loadPayments() {
    const user = getCustomer();

    if (!user) {
        return;
    }

    const paymentsElement =
        document.querySelector("#payments");

    if (!paymentsElement) {
        return;
    }

    try {
        const rows = await api(
            `/payments/customer/${user.id}`
        );

        if (!rows.length) {
            paymentsElement.innerHTML = `
                <div class="alert alert-info">
                    No payments.
                </div>
            `;

            return;
        }


        paymentsElement.innerHTML = `
            <div class="table-responsive">

                <table class="table">

                    <thead>

                        <tr>
                            <th>Booking</th>
                            <th>Amount</th>
                            <th>Status</th>
                            <th>Date</th>
                            <th>Action</th>
                        </tr>

                    </thead>


                    <tbody>

                        ${rows.map(payment => `

                            <tr>

                                <td>
                                    ${payment.booking?.bookingNumber
            || payment.bookingId
            }
                                </td>


                                <td>
                                    Rs. ${Number(
                payment.amount
            ).toFixed(2)}
                                </td>


                                <td>

                                    <span class="badge text-bg-success">
                                        ${payment.status}
                                    </span>

                                </td>


                                <td>
                                    ${new Date(
                payment.paidAt
            ).toLocaleString()
            }
                                </td>


                                <td>

                                    <a
                                        class="btn btn-sm btn-outline-primary"
                                        href="receipt.html?id=${payment.id}">

                                        Receipt

                                    </a>

                                </td>

                            </tr>

                        `).join("")}

                    </tbody>

                </table>

            </div>
        `;
    }
    catch (e) {
        paymentsElement.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;
    }
}


// ======================================================
// RECEIPT
// ======================================================

async function loadReceipt() {
    const user = getCustomer();

    if (!user) {
        return;
    }

    const receiptElement =
        document.querySelector("#receipt");

    if (!receiptElement) {
        return;
    }


    const id =
        new URLSearchParams(
            location.search
        ).get("id");


    if (!id) {
        receiptElement.innerHTML = `
            <div class="alert alert-warning">
                Receipt ID not found.
            </div>
        `;

        return;
    }


    try {
        const receiptData =
            await api(
                `/payments/${id}/receipt`
            );


        receiptElement.innerHTML = `
            <div class="card p-4">

                <div class="text-center mb-4">

                    <h2>
                        Payment Receipt
                    </h2>

                    <p class="text-muted mb-0">
                        EventParking Reservation System
                    </p>

                </div>

                <hr>


                <p>
                    <strong>Receipt:</strong>
                    ${receiptData.receiptNumber}
                </p>


                <p>
                    <strong>Booking:</strong>
                    ${receiptData.bookingNumber}
                </p>


                <p>
                    <strong>Event:</strong>
                    ${receiptData.eventName || ""}
                </p>


                <p>
                    <strong>Amount:</strong>
                    Rs. ${Number(
            receiptData.amount
        ).toFixed(2)}
                </p>


                <p>
                    <strong>Status:</strong>
                    ${receiptData.status}
                </p>


                <p>
                    <strong>Paid:</strong>
                    ${new Date(
            receiptData.paidAt
        ).toLocaleString()
            }
                </p>


                <button
                    class="btn btn-outline-dark"
                    onclick="window.print()">

                    Print Receipt

                </button>

            </div>
        `;
    }
    catch (e) {
        receiptElement.innerHTML = `
            <div class="alert alert-danger">
                ${e.message}
            </div>
        `;
    }
}


// ======================================================
// PAGE LOAD
// ======================================================

document.addEventListener(
    "DOMContentLoaded",
    () => {

        if (
            document.querySelector("#stats")
        ) {
            loadDashboard();
        }


        if (
            document.querySelector("#bookings")
        ) {
            loadBookings();
        }


        if (
            document.querySelector("#notifications")
        ) {
            loadNotifications();
        }


        if (
            document.querySelector("#payments")
        ) {
            loadPayments();
        }


        if (
            document.querySelector("#receipt")
        ) {
            loadReceipt();
        }

    }
);