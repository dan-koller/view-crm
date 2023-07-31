using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase
using View.Shared; // Ticket
using View.WebApi.Repositories; // ITicketRepository

namespace View.WebApi.Controllers;

// base address: api/ticket
[Route(Constants.ApiRoute)]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly ITicketRepository repository;

    public TicketController(ITicketRepository injectedRepository)
    {
        repository = injectedRepository;
    }

    // GET: api/ticket
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Ticket>))]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        IEnumerable<Ticket>? tickets = await repository.GetAllTicketAsync();
        if (tickets == null)
        {
            return NotFound();
        }
        return Ok(tickets);
    }

    // GET: api/ticket/<id>
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(Ticket))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Ticket>> GetTicket(long id)
    {
        Ticket? ticket = await repository.GetTicketAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }
        return Ok(ticket);
    }

    // POST: api/ticket
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Ticket))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Ticket>> PostTicket([FromBody] Ticket ticket)
    {
        if (ticket == null)
        {
            return BadRequest();
        }

        Ticket? newTicket = await repository.CreateTicketAsync(ticket);
        if (newTicket == null)
        {
            return BadRequest();
        }

        return CreatedAtAction(nameof(GetTicket), new { id = newTicket.Id }, newTicket);
    }

    // PUT: api/ticket/<id>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutTicket(long id, [FromBody] Ticket ticket)
    {
        if (ticket == null || ticket.Id != id)
        {
            return BadRequest();
        }

        Ticket? existingTicket = await repository.GetTicketAsync(id);
        if (existingTicket == null)
        {
            return NotFound();
        }

        await repository.UpdateTicketAsync(id, ticket);

        return NoContent();
    }

    // PATCH: api/ticket/<id>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PatchTicket(long id)
    {
        Ticket? existingTicket = await repository.GetTicketAsync(id);
        if (existingTicket == null)
        {
            return NotFound();
        }

        // If the ticket is marked as closed, then unmark it.
        existingTicket.Closed = existingTicket.Closed == true ? false : true;

        await repository.UpdateTicketAsync(id, existingTicket);

        return NoContent();
    }

    // DELETE: api/ticket/<id>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTicket(long id)
    {
        Ticket? existingTicket = await repository.GetTicketAsync(id);
        if (existingTicket == null)
        {
            return NotFound();
        }

        bool? deleted = await repository.DeleteTicketAsync(id);
        if (deleted.HasValue && deleted.Value) // short circuit AND
        {
            return new NoContentResult();
        }
        else
        {
            return BadRequest( // 400 Bad request
                    $"Ticket {id} was found but failed to delete.");
        }
    }
}

