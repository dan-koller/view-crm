using ViewBoard.Shared;

namespace ViewBoard.WebApi.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> CreateTicketAsync(Ticket ticket);
    Task<IEnumerable<Ticket>> GetAllTicketAsync();
    Task<Ticket?> GetTicketAsync(long id);
    Task<Ticket?> UpdateTicketAsync(long id, Ticket ticket);
    Task<bool?> DeleteTicketAsync(long id);
}

