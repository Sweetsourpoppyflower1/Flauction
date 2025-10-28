import React, { useState } from "react";
import axios from "axios";

function App() {
    const [form, setForm] = useState({
        Gebruikersnaam: "",
        Email: "",
        Wachtwoord: "",
        Rol: "Gebruiker"
    });
    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post("/api/gebruikers/register", form);
            setMessage(response.data || "Registratie gelukt!");
        } catch (err) {
            setMessage(err.response?.data || "Er is een fout opgetreden.");
        }
    };

    return (
        <div style={{ maxWidth: 400, margin: "auto", marginTop: 50 }}>
            <h2>Registreren</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Gebruikersnaam:</label>
                    <input
                        name="Gebruikersnaam"
                        value={form.Gebruikersnaam}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label>Email:</label>
                    <input
                        type="email"
                        name="Email"
                        value={form.Email}
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
                <button type="submit">Registreren</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
}

export default App;
