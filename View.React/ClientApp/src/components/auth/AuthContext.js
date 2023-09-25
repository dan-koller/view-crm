import React, { Component, useContext } from "react";

const AuthContext = React.createContext();

class AuthProvider extends Component {
    state = {
        user: null,
    };

    componentDidMount() {
        const user = localStorage.getItem("user");
        this.setState({ user });
    }

    getUser = () => {
        return JSON.parse(localStorage.getItem("user"));
    };

    userIsAuthenticated = () => {
        return localStorage.getItem("user") !== null;
    };

    userLogin = (user) => {
        localStorage.setItem("user", JSON.stringify(user));
        this.setState({ user });
    };

    userLogout = () => {
        localStorage.removeItem("user");
        this.setState({ user: null });
    };

    getAuthHeader = () => {
        const user = JSON.parse(localStorage.getItem("user"));
        const token = user && user.token;
        return token ? { Authorization: "Bearer " + token } : {};
    };

    render() {
        const { children } = this.props;
        const { user } = this.state;
        const {
            getUser,
            userIsAuthenticated,
            userLogin,
            userLogout,
            getAuthHeader,
        } = this;

        return (
            <AuthContext.Provider
                value={{
                    user,
                    getUser,
                    userIsAuthenticated,
                    userLogin,
                    userLogout,
                    getAuthHeader,
                }}
            >
                {children}
            </AuthContext.Provider>
        );
    }
}

export default AuthContext;

export function useAuth() {
    return useContext(AuthContext);
}

export { AuthProvider };
