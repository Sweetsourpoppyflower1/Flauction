import React from "react";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import Register from "./pages/register";
import Login from "./pages/login";
import Homepage from "./pages/homepage";

function App() {
    return (
        <Router>
            <nav style={{ display: "flex", gap: "10px", marginBottom: "20px" }}>
                <Link to="/register">Registreren</Link>
                <Link to="/login">Inloggen</Link>
                <Link to="/homepage">Homepage</Link>
            </nav>

            <Routes>
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/homepage" element={<Homepage />} />
            </Routes>
        </Router>
    );
}

export default App;
