import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../pages/styles/Login.css";

function Login() {
    const navigate = useNavigate();
    const [form, setForm] = useState({
        Gebruikersnaam: "",
        Wachtwoord: "",
    });

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        navigate('/homepage');
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
        </div>
    );
}

export default Login;
