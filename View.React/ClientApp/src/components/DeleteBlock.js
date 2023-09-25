import { api } from "../components/misc/Api";

const DeleteBlock = ({ ticketId, removeTicket }) => {
    const deleteTicket = async () => {
        let response = null;

        try {
            if (!removeTicket) {
                response = await api.patchTicket(ticketId);
            } else {
                response = await api.deleteTicket(ticketId);
            }
            const success = response.status == 204;
            if (success) window.location.reload();
        } catch (e) {
            // By default, axios throws an error if the response status is not 2xx
            // We can access the response object with e.response
            const status = e.response.status;
            if (status == 400) {
                alert("Error deleting ticket: " + e.response.data);
            }
        }
    };

    return (
        <div className='delete-block'>
            <div className='delete-icon' onClick={deleteTicket}>
                ✖
            </div>
        </div>
    );
};

export default DeleteBlock;
