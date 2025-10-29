import React, { useState } from "react";
import axios from "axios";
import "../pages/styles/Login.css"; // ⬅️ import hier

function Login() {
    const [form, setForm] = useState({
        Gebruikersnaam: "",
        Wachtwoord: "",
    });
    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post("/api/gebruikers/login", form);
            setMessage(response.data || "Inloggen gelukt!");
        } catch (err) {
            setMessage(err.response?.data || "Er is een fout opgetreden.");
        }
    };

    return (
        <div className="login-container">
            <h2>Inloggen</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Gebruikersnaam:</label>
                    <input
                        type="text"
                        name="Gebruikersnaam"
                        value={form.Gebruikersnaam}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label>Wachtwoord:</label>
                    <input
                        type="password"
                        name="Wachtwoord"
                        value={form.Wachtwoord}
                        onChange={handleChange}
                        required
                    />
                </div>
                <button type="submit">Inloggen</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
}

export default Login;