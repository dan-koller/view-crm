namespace View.UnitTests;
using View.Shared; // ViewContext

[Collection("Serial")]
public class EntityModelTests
{
    [Fact]
    public void TicketDatabaseConnectionTest()
    {
        using (ViewContext db = new())
        {
            // Create db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact]
    public void UserDatabaseConnectionTest()
    {
        using (UserContext db = new())
        {
            // Create db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    // Test ticket creation
    [Fact]
    public void TicketCreationTest()
    {
        using (ViewContext db = new())
        {
            // Create db if it doesn't exist
            db.Database.EnsureCreated();

            // Create a ticket
            Ticket ticket = new()
            {
                Category = "Test Category",
                Color = "Test Color",
                Title = "Test Ticket",
                Owner = "Test Owner",
                Avatar = "Test Avatar",
                Status = "Test Status",
                Priority = 1,
                Progress = 0,
                Description = "Test Description",
                Timestamp = DateTime.Now.ToString(),
                Notes = "Test Notes",
                Closed = false
            };

            // Add ticket to db
            db.Tickets.Add(ticket);
            db.SaveChanges();

            // Check if ticket exists
            Assert.True(db.Tickets.Any(t => t.Title == "Test Ticket"));
        }
    }

    // Test ticket deletion
    [Fact]
    public void TicketDeletionTest()
    {
        using (ViewContext db = new())
        {
            // Delete the ticket created in the previous test
            Ticket? ticket = db.Tickets.FirstOrDefault(t => t.Title == "Test Ticket");
            if (ticket != null)
            {
                db.Tickets.Remove(ticket);
                db.SaveChanges();

            }

            // Check if ticket exists
            Assert.False(db.Tickets.Any(t => t.Title == "Test Ticket"));
        }
    }
}