import axios from "axios";
import { config } from "../../Constants";

export const api = {
    // Authentication
    authenticate,
    register,
    updateAccount,
    // Administrator
    // RegisteredUser
    fetchTickets,
    updateTicket,
    postTicket,
    fetchTicket,
    patchTicket,
    deleteTicket,
};

// Authenticate user
function authenticate(email, password) {
    return instance.post(
        "/api/account/login",
        { email, password },
        { headers: { "Content-type": "application/json" } }
    );
}

// Register new user
function register(name, email, password) {
    return instance.post(
        "/api/account/register",
        { name, email, password },
        { headers: { "Content-type": "application/json" } }
    );
}

// Update account
function updateAccount(name, email, currentPassword, newPassword) {
    return instance.post(
        "/api/account/update",
        { name, email, currentPassword, newPassword },
        { headers: { Authorization: bearerAuth() } }
    );
}

// -- Administrative tasks --

/* CURRENTLY NOT IMPLEMENTED */

// -- RegisteredUser tasks --
// Dashboard
async function fetchTickets() {
    return await instance.get("/api/ticket", {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

// TicketPage
async function updateTicket(formdata, id) {
    return await instance.put(`/api/ticket/${id}`, formdata, {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

async function postTicket(formdata) {
    return await instance.post("/api/ticket", formdata, {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

async function fetchTicket(id) {
    return await instance.get(`/api/ticket/${id}`, {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

// DeleteBlock
async function patchTicket(id) {
    return await instance.patch(`/api/ticket/${id}`, null, {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

async function deleteTicket(id) {
    return await instance.delete(`/api/ticket/${id}`, {
        headers: {
            Authorization: bearerAuth(),
        },
    });
}

// -- Axios instance --

// Create axios instance and set default config
const instance = axios.create({
    baseURL: config.url.API_BASE_URL,
});

// -- Helper functions --

// Handle bearer authentication for all requests
function bearerAuth() {
    const user = JSON.parse(localStorage.getItem("user"));
    const token = user && user.token;
    return `Bearer ${token}`;
}
