import axios from 'axios'

const DeleteBlock = ({ ticketId }) => {

    const deleteTicket = async () => {
        const response = await axios.delete(`https://localhost:5002/api/ticket/${ticketId}`);
        const success = response.status == 204;
        if (success) window.location.reload();
    }

    return (
        <div className="delete-block">
            <div className="delete-icon" onClick={deleteTicket}>✖</div>
        </div>
    )
}

export default DeleteBlock;