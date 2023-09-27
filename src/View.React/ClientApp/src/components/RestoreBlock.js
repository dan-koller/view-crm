import { api } from "../components/misc/Api";

const RestoreBlock = ({ ticketId }) => {
    const restoreTicket = async () => {
        const response = await api.patchTicket(ticketId);
        const success = response.status == 204;
        if (success) window.location.reload();
    };

    return (
        <div className='restore-block'>
            <div className='restore-icon' onClick={restoreTicket}>
                🗘
            </div>
        </div>
    );
};

export default RestoreBlock;
