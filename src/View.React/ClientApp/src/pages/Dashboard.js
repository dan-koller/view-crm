import { useContext, useEffect, useState } from "react";
import TicketCard from "../components/TicketCard";
import CategoriesContext from "../context";
import { api } from "../components/misc/Api";
import { Input } from "reactstrap";

const Dashboard = ({ isClosedPage }) => {
    const [tickets, setTickets] = useState(null);
    const { categories, setCategories } = useContext(CategoriesContext);

    useEffect(() => {
        async function fetchData() {
            try {
                const response = await api.fetchTickets(); // Api.js already handles the bearerAuth
                const dataObject = response.data.filter((ticket) =>
                    isClosedPage ? ticket.closed : !ticket.closed
                );

                if (dataObject) {
                    const arrayOfKeys = Object.keys(dataObject);
                    const arrayOfData = Object.values(dataObject);
                    const formattedArray = arrayOfKeys.map((key, index) => {
                        const formattedData = {
                            ...arrayOfData[index],
                            documentId: key,
                        };
                        return formattedData;
                    }).sort((a, b) => a.id - b.id); // Sort ascending by id
                    setTickets(formattedArray);
                }
            } catch (error) {
                // Handle the error
                console.warn(error);
            }
        }
        fetchData();
    }, [isClosedPage]);

    useEffect(() => {
        setCategories([...new Set(tickets?.map(({ category }) => category))]);
    }, [tickets]);

    const colors = [
        "rgb(255,179,186)",
        "rgb(255,223,186)",
        "rgb(255,255,186)",
        "rgb(186,255,201)",
        "rgb(186,225,255)",
    ];

    const uniqueCategories = [
        ...new Set(tickets?.map(({ category }) => category)),
    ];

    const [searchQuery, setSearchQuery] = useState("");

    // Function to filter tickets by title
    const filteredTickets = tickets
        ? tickets.filter((ticket) =>
            ticket.title.toLowerCase().includes(searchQuery.toLowerCase())
        )
        : [];

    return (
        <div className='dashboard'>
            <h1>{isClosedPage ? "Closed projects" : "My Projects"}</h1>

            <Input
                type='text'
                placeholder='Search by title...'
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
            />

            <div className='ticket-container'>
                {tickets &&
                    uniqueCategories?.map((uniqueCategory, categoryIndex) => (
                        <div key={categoryIndex}>
                            <h3>{uniqueCategory}</h3>
                            {filteredTickets
                                .filter(
                                    (ticket) =>
                                        ticket.category === uniqueCategory
                                )
                                .map((filteredTicket, _index) => (
                                    <TicketCard
                                        key={filteredTicket.id} // Adding a unique key here
                                        id={_index}
                                        color={
                                            colors[categoryIndex] || colors[0]
                                        }
                                        ticket={filteredTicket}
                                    />
                                ))}
                        </div>
                    ))}
            </div>
        </div>
    );
};

export default Dashboard;
