import React from "react";
import { useNavigate } from "react-router-dom";
import "./styles/homepage.css";

function Homepage() {
    const navigate = useNavigate();

    // Dummy data for products with images
    const dummyProducts = [
        {
            id: 1,
            naam: "Chrysanthemum",
            beschrijving: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            image: "/assets/download.jpg"
        },
        {
            id: 2,
            naam: "Hortensia",
            beschrijving: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            image: "/assets/51M1pKcDpQL.jpg"
        },
        {
            id: 3,
            naam: "Tulip",
            beschrijving: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            image: "/assets/images.jpg"
        },
        {
            id: 4,
            naam: "Hyacinth",
            beschrijving: "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            image: "/assets/2545033_Atmosphere_01_SQ_c8n0uw.webp"
        }
    ];

    // Get active and next products
    const activeProduct = dummyProducts[0];
    const nextProducts = dummyProducts.slice(1, 4);

    return (
        <div className="homepage-container">
            {/* Left Column - Active Auction Product */}
            <div className="homepage-column left-column">
                <h3 className="column-title">Active Auction Product</h3>
                <div className="product-card active-product">
                    <div className="product-image-container">
                        <img src={activeProduct.image} alt={activeProduct.naam} className="product-image" />
                    </div>
                    <div className="product-info">
                        <h4 className="product-name">{activeProduct.naam}</h4>
                        <p className="product-description">
                            {activeProduct.beschrijving}
                        </p>
                    </div>
                </div>
            </div>

            {/* Center Column - Welcome Section */}
            <div className="homepage-column center-column">
                <div className="welcome-section">
                    <p className="welcome-text">
                        Welcome to the HomePage of Flauction; the best Flora Auction Clock to date! Navigate to the Auction from here, or log out.
                    </p>
                    
                    <div className="button-section">
                        <h4 className="button-title">Go to Auction</h4>
                        <p className="button-subtitle">Click below to go to the live Auction</p>
                        <button className="auction-button" onClick={() => navigate('/auction')}>
                            AUCTION
                        </button>
                    </div>

                    <div className="button-section">
                        <p className="button-subtitle">Click below to view the next Products</p>
                        <button className="next-products-button" onClick={() => navigate('/products')}>
                            NEXT PRODUCTS
                        </button>
                    </div>
                </div>
            </div>

            {/* Right Column - Next Auction Products */}
            <div className="homepage-column right-column">
                <h3 className="column-title">Next Auction Products</h3>
                <div className="next-products-list">
                    {nextProducts.map((product) => (
                        <div key={product.id} className="product-card next-product">
                            <div className="product-image-container-small">
                                <img src={product.image} alt={product.naam} className="product-image-small" />
                            </div>
                            <div className="product-info-small">
                                <h4 className="product-name-small">{product.naam}</h4>
                                <p className="product-description-small">
                                    {product.beschrijving}
                                </p>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default Homepage;
