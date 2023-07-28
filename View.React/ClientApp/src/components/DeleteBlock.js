import axios from 'axios';

const DeleteBlock = ({ ticketId, removeTicket }) => {

    const deleteTicket = async () => {
        let response = null;
        if (!removeTicket) {
            response = await axios.patch(`https://localhost:5002/api/ticket/${ticketId}`);
        }
        else {

            response = await axios.delete(`https://localhost:5002/api/ticket/${ticketId}`);
        }
        const success = response.status == 204;
        if (success) window.location.reload();
    }

    return (
        <div className="delete-block">
            <div className="delete-icon" onClick={deleteTicket}>✖</div>
        </div>
    );
}

export default DeleteBlock;