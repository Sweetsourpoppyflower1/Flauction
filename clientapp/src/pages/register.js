import React, { useState } from "react";
import axios from "axios";

function Register() {
    const [form, setForm] = useState({
        CompanyName: "",
        CompanyAddress: "",
        PostalCode: "",
        Country: "",
        ContactPersonName: "",
        ContactPersonPhoneNumber: "",
        ContactPersonEmail: "",
        VatNumber: "",
        IBAN: "",
        BICSWIFT: "",
        Password: "",
        ConfirmPassword: "",
        TermsAndConditions: false,
        Rol: "Bedrijf",
    });
    const [message, setMessage] = useState("");

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        const newValue = type === "checkbox" ? checked : value;
        setForm((prev) => ({ ...prev, [name]: newValue }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        // Basic client-side validation
        if (form.Password !== form.ConfirmPassword) {
            setMessage("Passwords do not match.");
            return;
        }
        if (!form.TermsAndConditions) {
            setMessage("You must accept the terms and conditions.");
            return;
        }

        // Prepare payload (omit ConfirmPassword if you don't want to send it)
        const payload = { ...form };
        delete payload.ConfirmPassword;

        try {
            const response = await axios.post("/api/gebruikers/register", payload);
            setMessage(response.data || "Registratie gelukt!");
        } catch (err) {
            setMessage(err.response?.data || "Er is een fout opgetreden.");
        }
    };

    return (
        <div style={{ maxWidth: 600, margin: "auto", marginTop: 50 }}>
            <h2>Registreren als bedrijf</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Company Name:</label>
                    <input
                        name="CompanyName"
                        value={form.CompanyName}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Company Address:</label>
                    <input
                        name="CompanyAddress"
                        value={form.CompanyAddress}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Postal Code:</label>
                    <input
                        name="PostalCode"
                        value={form.PostalCode}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Country:</label>
                    <select
                        name="Country"
                        value={form.Country}
                        onChange={handleChange}
                        required
                    >
                        <option value="">Select country</option>
                        <option value="Netherlands">Netherlands</option>
                        <option value="Belgium">Belgium</option>
                        <option value="Luxembourg">Luxembourg</option>
                        <option value="Germany">Germany</option>
                        <option value="France">France</option>
                        <option value="United Kingdom">United Kingdom</option>
                        <option value="United States">United States</option>
                        <option value="Other">Other</option>
                    </select>
                </div>

                <div>
                    <label>Contact Person Name:</label>
                    <input
                        name="ContactPersonName"
                        value={form.ContactPersonName}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Contact Person Phone Number:</label>
                    <input
                        type="tel"
                        name="ContactPersonPhoneNumber"
                        value={form.ContactPersonPhoneNumber}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Contact Person Email:</label>
                    <input
                        type="email"
                        name="ContactPersonEmail"
                        value={form.ContactPersonEmail}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>VAT Number:</label>
                    <input
                        name="VatNumber"
                        value={form.VatNumber}
                        onChange={handleChange}
                    />
                </div>

                <div>
                    <label>IBAN:</label>
                    <input
                        name="IBAN"
                        value={form.IBAN}
                        onChange={handleChange}
                    />
                </div>

                <div>
                    <label>BIC / SWIFT:</label>
                    <input
                        name="BICSWIFT"
                        value={form.BICSWIFT}
                        onChange={handleChange}
                    />
                </div>

                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        name="Password"
                        value={form.Password}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>Confirm Password:</label>
                    <input
                        type="password"
                        name="ConfirmPassword"
                        value={form.ConfirmPassword}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div>
                    <label>
                        <input
                            type="checkbox"
                            name="TermsAndConditions"
                            checked={form.TermsAndConditions}
                            onChange={handleChange}
                        />{" "}
                        I accept the terms and conditions
                    </label>
                </div>

                <button type="submit">Registreren</button>
            </form>

            {message && <p>{message}</p>}
        </div>
    );
}

export default Register;