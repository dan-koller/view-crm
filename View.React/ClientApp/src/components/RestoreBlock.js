import axios from 'axios';

const RestoreBlock = ({ ticketId }) => {

    const restoreTicket = async () => {
        const response = await axios.patch(`https://localhost:5002/api/ticket/${ticketId}`);
        const success = response.status == 204;
        if (success) window.location.reload();
    }

    return (
        <div className="restore-block">
            <div className="restore-icon" onClick={restoreTicket}>🗘</div>
        </div>
    );
}

export default RestoreBlock;