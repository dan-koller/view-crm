import axios from 'axios';
import { useContext, useEffect, useState } from 'react';
import TicketCard from '../components/TicketCard';
import CategoriesContext from '../context';

const DashBoard = ({ isClosedPage }) => {
    const [tickets, setTickets] = useState(null);
    const { categories, setCategories } = useContext(CategoriesContext);

    useEffect(() => {
        async function fetchData() {
            try {
                const response = await axios.get('https://localhost:5002/api/ticket');
                const dataObject = response.data.filter(
                    (ticket) => isClosedPage ? ticket.closed : !ticket.closed
                );

                if (dataObject) {
                    const arrayOfKeys = Object.keys(dataObject);
                    const arrayOfData = Object.values(dataObject);
                    const formattedArray = arrayOfKeys.map((key, index) => {
                        const formattedData = { ...arrayOfData[index], documentId: key };
                        return formattedData;
                    });
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
        setCategories([...new Set(tickets?.map(({ category }) => category))])
    }, [tickets]);

    const colors = [
        'rgb(255,179,186)',
        'rgb(255,223,186)',
        'rgb(255,255,186)',
        'rgb(186,255,201)',
        'rgb(186,225,255)',
    ];

    const uniqueCategories = [
        ...new Set(tickets?.map(({ category }) => category)),
    ];

    return (
        <div className="dashboard">
            <h1>{isClosedPage ? 'Closed projects' : 'My Projects'}</h1>
            <div className="ticket-container">
                {tickets &&
                    uniqueCategories?.map((uniqueCategory, categoryIndex) => (
                        <div key={categoryIndex}>
                            <h3>{uniqueCategory}</h3>
                            {tickets
                                .filter((ticket) => ticket.category === uniqueCategory)
                                .map((filteredTicket, _index) => (
                                    <TicketCard
                                        key={filteredTicket.id} // Adding a unique key here
                                        id={_index}
                                        color={colors[categoryIndex] || colors[0]}
                                        ticket={filteredTicket}
                                    />
                                ))}
                        </div>
                    ))}
            </div>
        </div>
    );
}

export default DashBoard;
