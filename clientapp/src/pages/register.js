import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../pages/styles/register.css";

function Register() {
    const navigate = useNavigate();

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

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        const newValue = type === "checkbox" ? checked : value;
        setForm((prev) => ({ ...prev, [name]: newValue }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Navigate directly to login without validation
        navigate("/login");
    };

    return (
        <div className="register-page">
            <div className="register-container">
                <h2>Register</h2>
                <form onSubmit={handleSubmit}>
                    <div>
                        <label>Company Name:</label>
                        <input
                            type="text"
                            name="CompanyName"
                            value={form.CompanyName}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div>
                        <label>Company Address:</label>
                        <input
                            type="text"
                            name="CompanyAddress"
                            value={form.CompanyAddress}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="register-grid two-column">
                        <div>
                            <label>Postal Code:</label>
                            <input
                                type="text"
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
                            </select>
                        </div>
                    </div>

                    <div>
                        <label>Contact Person Name:</label>
                        <input
                            type="text"
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
                            type="text"
                            name="VatNumber"
                            value={form.VatNumber}
                            onChange={handleChange}
                        />
                    </div>

                    <div>
                        <label>IBAN:</label>
                        <input
                            type="text"
                            name="IBAN"
                            value={form.IBAN}
                            onChange={handleChange}
                        />
                    </div>

                    <div>
                        <label>BIC / SWIFT:</label>
                        <input
                            type="text"
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
                        <label className="checkbox-label">
                            <input
                                type="checkbox"
                                name="TermsAndConditions"
                                checked={form.TermsAndConditions}
                                onChange={handleChange}
                            />
                            I accept the terms and conditions
                        </label>
                    </div>

                    <button type="submit">Registreren</button>
                </form>
            </div>
        </div>
    );
}

export default Register;
