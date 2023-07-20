import { Route, Routes, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import AppRoutes from './AppRoutes';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import { Layout } from './components/Layout';
import './custom.css';
import CategoriesContext from './context';
import Nav from './components/Nav';

const App = () => {
    const displayName = App.name;

    const [categories, setCategories] = useState(null);
    const value = { categories, setCategories };
    const location = useLocation();

    useEffect(() => {
        // If the path is /dashboard or /ticket, display the nav
        const isDashboardOrTicketRoute =
            location.pathname === '/dashboard' || location.pathname === '/ticket';
        const shouldDisplayNav = isDashboardOrTicketRoute;

        // Update the nav visibility in the state
        setNavVisibility(shouldDisplayNav);
    }, [location]);

    const [navVisibility, setNavVisibility] = useState(false);

    return (
        <Layout>
            <div className="app">
                <CategoriesContext.Provider value={value}>
                    {navVisibility && <Nav />}
                    <Routes>
                        {AppRoutes.map((route, index) => {
                            const { element, requireAuth, ...rest } = route;
                            return (
                                <Route
                                    key={index}
                                    {...rest}
                                    element={
                                        requireAuth ? (
                                            <AuthorizeRoute {...rest} element={element} />
                                        ) : (
                                            element
                                        )
                                    }
                                />
                            );
                        })}
                    </Routes>
                </CategoriesContext.Provider>
            </div>
        </Layout>
    );
};

export default App;
