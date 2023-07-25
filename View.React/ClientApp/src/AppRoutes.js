import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import Dashboard  from "./pages/Dashboard";
import TicketPage from "./pages/TicketPage";
import { Home } from "./components/Home";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/dashboard',
        requireAuth: true,
        element: <Dashboard />
    },
    {
        path: '/ticket',
        requireAuth: true,
        element: <TicketPage />
    },
    {
        path: '/ticket/:id',
        requireAuth: true,
        element: <TicketPage editMode={true} />
    },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
