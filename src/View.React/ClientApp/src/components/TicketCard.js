import { Link } from 'react-router-dom'
import PriorityDisplay from './PriorityDisplay'
import ProgressDisplay from './ProgressDisplay'
import StatusDisplay from './StatusDisplay'
import AvatarDisplay from './AvatarDisplay'
import DeleteBlock from './DeleteBlock'
import RestoreBlock from './RestoreBlock'

const TicketCard = ({ color, ticket }) => {
    return (
        <div className="ticket-card">
            <div className="ticket-color" style={{ backgroundColor: color }}></div>
            <Link to={`/ticket/${ticket.id}`} id="link">
                <h3>{ticket.title}</h3>
                <AvatarDisplay ticket={ticket} />
                <StatusDisplay status={ticket.status} />
                <PriorityDisplay priority={Number(ticket.priority)} />
                <ProgressDisplay progress={Number(ticket.progress)} />
            </Link>
            {/*If the ticket is flagged as closed, also display the RestoreBlock component*/}
            {ticket.closed && <RestoreBlock ticketId={ticket.id} />}
            <DeleteBlock ticketId={ticket.id} removeTicket={ticket.closed} />
        </div>
    );
}

export default TicketCard;