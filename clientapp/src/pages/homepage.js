import React, { useState, useEffect } from "react";
import axios from "axios";

function Homepage() {
    const [veilingsproducten, setVeilingsproducten] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");


    useEffect(() => {
        const fetchVeilingsproducten = async () => {
            try {
                const response = await axios.get("/api/veilingsproducten");
                setVeilingsproducten(response.data);
                setLoading(false);
            } catch (err) {
                setError("Fout bij het ophalen van veilingsproducten.");
                setLoading(false);
            }
        };

        fetchVeilingsproducten();
    }, []);

    if (loading) return <div>Laden...</div>;
    if (error) return <div style={{ color: "red" }}>{error}</div>;

    return (
        <div style={{ maxWidth: 800, margin: "auto", marginTop: 50 }}>
            <h2>Veilingsproducten</h2>
            {veilingsproducten.length === 0 ? (
                <p>Geen veilingsproducten beschikbaar.</p>
            ) : (
                <div style={{ display: "grid", gap: "20px" }}>
                    {veilingsproducten.map((product) => (
                        <div
                            key={product.id}
                            style={{
                                border: "1px solid #ccc",
                                padding: "15px",
                                borderRadius: "5px",
                            }}
                        >
                            <h3>{product.naam}</h3>
                            <p>{product.beschrijving}</p>
                            <p><strong>Minimumprijs:</strong> €{product.minimumprijs}</p>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default Homepage;
