import React from "react";
import { Route, Routes } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { Layout } from "./components/Layout";
import "./custom.css";
import Login from "./components/auth/Login";
import { AuthProvider } from "./components/auth/AuthContext";
import { useState } from "react";
import { useLocation } from "react-router-dom";
import { useEffect } from "react";
import Sidenav from "./components/Sidenav";
import CategoriesContext from "./context";

const App = () => {
    const displayName = App.name;

    const [categories, setCategories] = useState(null);
    const value = { categories, setCategories };
    const location = useLocation();

    useEffect(() => {
        // If the path is /dashboard or /ticket, display the nav
        const isDashboardOrTicketRoute =
            location.pathname === "/dashboard" ||
            location.pathname === "/dashboard/closed" ||
            /^\/ticket(\/\d+)?$/.test(location.pathname); // Check if the path is /ticket or /ticket/:id
        const shouldDisplayNav = isDashboardOrTicketRoute;

        // Update the nav visibility in the state
        setNavVisibility(shouldDisplayNav);
    }, [location]);

    const [navVisibility, setNavVisibility] = useState(false);

    const userIsAuthenticated = () => {
        return localStorage.getItem("user") !== null;
    };

    return (
        <AuthProvider>
            <Layout>
                <div className='app'>
                    {/* If any errors occur here, remove CategoriesContext (not sure, if truly needed)*/}
                    <CategoriesContext.Provider value={value}>
                        {navVisibility && <Sidenav />}
                        <Routes>
                            {AppRoutes.map((route, index) => {
                                const { element, requireAuth, ...rest } = route;
                                return requireAuth && !userIsAuthenticated() ? (
                                    <Route
                                        key={index}
                                        {...rest}
                                        element={<Login />}
                                    />
                                ) : (
                                    <Route
                                        key={index}
                                        {...rest}
                                        element={element}
                                    />
                                );
                            })}
                        </Routes>
                    </CategoriesContext.Provider>
                </div>
            </Layout>
        </AuthProvider>
    );
};

export default App;
