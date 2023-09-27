using Microsoft.EntityFrameworkCore.ChangeTracking; // EntityEntry<T>
using View.Shared; // Ticket
using System.Collections.Concurrent; // ConcurrentDictionary

namespace View.WebApi.Repositories;

public class TicketRepository : ITicketRepository
{
    // Use a static thread-safe dictionary to cache the tickets.
    private static ConcurrentDictionary<long, Ticket>? ticketsCache;

    // Use an instance data context field because it should not be
    // cached due to the data context having internal caching.
    private ViewContext db;

    public TicketRepository(ViewContext injectedContext)
    {
        db = injectedContext;

        // Preload codes from database as normal
        // Dictionary with Guid as the key,
        // then convert to a thread-safe ConcurrentDictionary
        if (ticketsCache == null)
        {
            ticketsCache = new ConcurrentDictionary<long, Ticket>(
                db.Tickets.ToDictionary(c => c.Id)
            );
        }
    }

    public async Task<Ticket?> CreateTicketAsync(Ticket ticket)
    {
        EntityEntry<Ticket>? added = await db.Tickets.AddAsync(ticket);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            if (ticketsCache is null) return ticket;
            // If the code is new, add it to cache,
            // else call UpdateCache method
            return ticketsCache.AddOrUpdate(ticket.Id, ticket, UpdateCache);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Ticket>> GetAllTicketAsync()
    {
        // For performance, get from cache.
        return Task.FromResult(ticketsCache is null
            ? Enumerable.Empty<Ticket>() : ticketsCache.Values);
    }

    public Task<Ticket?> GetTicketAsync(long id)
    {
        if (ticketsCache is null) return null!;
        ticketsCache.TryGetValue(id, out Ticket? ticket);
        return Task.FromResult(ticket);
    }

    public async Task<Ticket?> UpdateTicketAsync(long id, Ticket ticket)
    {
        // Update the ticket in the database
        db.Tickets.Update(ticket);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            // Update in cache.
            return UpdateCache(id, ticket);
        }
        return null;
    }

    public async Task<bool?> DeleteTicketAsync(long id)
    {
        Ticket? t = db.Tickets.Find(id);
        if (t is null) return null;
        // Remove from database.
        db.Tickets.Remove(t);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            if (ticketsCache is null) return null;
            // Remove from cache.
            return ticketsCache.TryRemove(id, out t);
        }
        else
        {
            return null;
        }
    }

    // Helper method to update the cache
    private Ticket UpdateCache(long id, Ticket t)
    {
        Ticket? old;
        if (ticketsCache is not null)
        {
            if (ticketsCache.TryGetValue(id, out old))
            {
                if (ticketsCache.TryUpdate(id, t, old))
                {
                    return t;
                }
            }
        }
        return null!;
    }
}

