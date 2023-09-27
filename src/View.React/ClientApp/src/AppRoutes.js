import { Home } from "./components/Home";
import Login from "./components/auth/Login";
import Signup from "./components/auth/Signup";
import AccountPage from "./pages/AccountPage";
import Dashboard from "./pages/Dashboard";
import TicketPage from "./pages/TicketPage";

const AppRoutes = [
    {
        index: true,
        element: <Home />,
    },
    {
        path: "/login",
        element: <Login />,
    },
    {
        path: "/register",
        element: <Signup />,
    },
    {
        path: "/account",
        requireAuth: true,
        element: <AccountPage />,
    },
    {
        path: "/dashboard",
        requireAuth: true,
        element: <Dashboard />,
    },
    {
        path: "/dashboard/closed",
        requireAuth: true,
        element: <Dashboard isClosedPage={true} />,
    },
    {
        path: "/ticket",
        requireAuth: true,
        element: <TicketPage />,
    },
    {
        path: "/ticket/:id",
        requireAuth: true,
        element: <TicketPage editMode={true} />,
    },
];

export default AppRoutes;
