const API = "/api";

function currentUser() {
    try {
        return JSON.parse(localStorage.getItem("user") || "null");
    }
    catch {
        return null;
    }
}

async function api(path, options = {}) {
    const user = currentUser();

    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {})
    };

    if (user?.token || user?.Token) {
        headers.Authorization = `Bearer ${user.token || user.Token}`;
    }

    const res = await fetch(API + path, {
        ...options,
        headers
    });

    const text = await res.text();
    let data = null;

    try {
        data = text ? JSON.parse(text) : null;
    }
    catch {
        data = text;
    }

    if (!res.ok) {
        if (res.status === 401) {
            localStorage.removeItem("user");
        }

        throw new Error(
            data?.message ||
            data ||
            `Request failed (${res.status})`
        );
    }

    return data;
}

function logout() {
    localStorage.removeItem("user");
    localStorage.removeItem("verifyToken");
    localStorage.removeItem("resetToken");
    location.href = "login.html";
}
