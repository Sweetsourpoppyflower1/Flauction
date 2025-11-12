import React from "react";
import { Link } from "react-router-dom";
import "./Navbar.css";

function Navbar() {
    return (
        <nav className="navbar">
            <div className="navbar-container">
                <div className="navbar-logo">
                    <img src="/logo.png" alt="Flauction Logo" className="navbar-logo-img" />
                </div>
                <div className="navbar-links">
                    <Link to="/homepage" className="navbar-link">Homepage</Link>
                    <Link to="/login" className="navbar-link">Inloggen</Link>
                    <Link to="/register" className="navbar-link">Registreren</Link>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
