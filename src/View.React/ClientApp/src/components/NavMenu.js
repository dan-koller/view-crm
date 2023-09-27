import React, { useState } from "react";
import {
    Collapse,
    Navbar,
    NavbarBrand,
    NavbarToggler,
    NavItem,
    NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";
import { useAuth } from "./auth/AuthContext";

function NavMenu() {
    const [collapsed, setCollapsed] = useState(true);
    const auth = useAuth();

    const toggleNavbar = () => {
        setCollapsed(!collapsed);
    };

    const logout = () => {
        auth.userLogout();
    };

    const enterMenuStyle = () => {
        return auth.userIsAuthenticated()
            ? { display: "none" }
            : { display: "block" };
    };

    const logoutMenuStyle = () => {
        return auth.userIsAuthenticated()
            ? { display: "block" }
            : { display: "none" };
    };

    const adminPageStyle = () => {
        const user = auth.getUser();
        return user && user.role === "Administrator"
            ? { display: "block" }
            : { display: "none" };
    };

    const userPageStyle = () => {
        const user = auth.getUser();
        return user &&
            // Admins also have the RegisteredUser role
            (user.role === "RegisteredUser" ||
                user.role.includes("Administrator"))
            ? { display: "block" }
            : { display: "none" };
    };

    const getUserName = () => {
        const user = auth.getUser();
        return user ? user.name : "";
    };

    const getGreeting = () => {
        const currentTime = new Date().getHours();

        if (currentTime >= 6 && currentTime < 12) {
            return "Good morning"; // 6am - 12pm
        }
        if (currentTime >= 12 && currentTime < 18) {
            return "Good afternoon"; // 12pm - 6pm
        }
        if (currentTime >= 18 || currentTime < 6) {
            return "Good evening"; // 6pm - 6am
        }
    };

    return (
        <header>
            <Navbar
                className='navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3'
                container
                light
            >
                <NavbarBrand tag={Link} to='/'>
                    View CRM
                </NavbarBrand>
                <NavbarToggler onClick={toggleNavbar} className='mr-2' />
                <Collapse
                    className='d-sm-inline-flex flex-sm-row-reverse'
                    isOpen={!collapsed}
                    navbar
                >
                    <ul className='navbar-nav flex-grow'>
                        <NavItem>
                            <NavLink tag={Link} className='text-dark' to='/'>
                                Home
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/register'
                                style={enterMenuStyle()}
                            >
                                Register
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/login'
                                style={enterMenuStyle()}
                            >
                                Login
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/dashboard'
                                style={userPageStyle()}
                            >
                                Dashboard
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/ticket'
                                style={userPageStyle()}
                            >
                                Ticket
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/account' // Navigate back to home page
                                style={userPageStyle()}
                            >
                                {getGreeting()}, {getUserName()}
                            </NavLink>
                        </NavItem>
                        <NavItem>
                            <NavLink
                                tag={Link}
                                className='text-dark'
                                to='/' // Navigate back to home page
                                style={logoutMenuStyle()}
                                onClick={logout}
                            >
                                Logout
                            </NavLink>
                        </NavItem>
                    </ul>
                </Collapse>
            </Navbar>
        </header>
    );
}

export default NavMenu;
