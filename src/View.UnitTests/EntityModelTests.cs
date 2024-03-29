using View.Shared; // ViewContext, UserContext, Ticket

namespace View.UnitTests;

/*
 * Tests are performed using xUnit.net. However, to run the tests in a specific
 * order, a custom TestCaseOrderer and priority attribute are used. In this
 * case, it is easier to test individual methods in the order they are written,
 * rather than writing tests for each method and then running them in a random
 * order. After all, it is just a side project :)
 */

// [Collection("Sequential")]
[TestCaseOrderer("View.UnitTests.PriorityOrderer", "View.UnitTests")]
public class EntityModelTests
{
    [Fact, TestPriority(1)]
    public void TestTicketCreation()
    {
        using (ViewContext db = new())
        {
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
                // Timestamp not necessary for testing
                // & this supports multiple db models
                // (e.g. Sqlite and SqlServer)
                Timestamp = null,
                Notes = "Test Notes",
                Closed = false
            };

            db.Tickets.Add(ticket);
            db.SaveChanges();

            Assert.True(db.Tickets.Any(t => t.Title == "Test Ticket"));
        }
    }

    [Fact, TestPriority(2)]
    public void TestTicketUpdate()
    {
        using (ViewContext db = new())
        {
            // Update the ticket created in the previous test
            Ticket? ticket = db.Tickets.FirstOrDefault(t => t.Title == "Test Ticket");
            if (ticket != null)
            {
                ticket.Category = "Updated Test Category";
                ticket.Color = "Updated Test Color";
                ticket.Title = "Updated Test Ticket";
                ticket.Owner = "Updated Test Owner";
                ticket.Avatar = "Updated Test Avatar";
                ticket.Status = "Updated Test Status";
                ticket.Priority = 2;
                ticket.Progress = 1;
                ticket.Description = "Updated Test Description";
                ticket.Notes = "Updated Test Notes";
                ticket.Closed = true;

                db.Tickets.Update(ticket);
                db.SaveChanges();
            }

            // Check if all fields have been updated correctly
            Assert.True(db.Tickets.Any(t => t.Category == "Updated Test Category"));
            Assert.True(db.Tickets.Any(t => t.Color == "Updated Test Color"));
            Assert.True(db.Tickets.Any(t => t.Title == "Updated Test Ticket"));
            Assert.True(db.Tickets.Any(t => t.Owner == "Updated Test Owner"));
            Assert.True(db.Tickets.Any(t => t.Avatar == "Updated Test Avatar"));
            Assert.True(db.Tickets.Any(t => t.Status == "Updated Test Status"));
            Assert.True(db.Tickets.Any(t => t.Priority == 2));
            Assert.True(db.Tickets.Any(t => t.Progress == 1));
            Assert.True(db.Tickets.Any(t => t.Description == "Updated Test Description"));
            Assert.True(db.Tickets.Any(t => t.Notes == "Updated Test Notes"));
            Assert.True(db.Tickets.Any(t => t.Closed == true));
        }
    }

    [Fact, TestPriority(3)]
    public void TestTicketUpdateSuccess()
    {
        using (ViewContext db = new())
        {
            // 'Old' ticket should not exist anymore
            Assert.False(db.Tickets.Any(t => t.Title == "Test Ticket"));
        }
    }

    [Fact, TestPriority(4)]
    public void TestTicketFiltering()
    {
        using (ViewContext db = new())
        {
            // Filter tickets by category
            var categoryATickets = db.Tickets.Where(t => t.Category == "Updated Test Category").ToList();

            // Validate that only tickets with the specified category are retrieved
            Assert.True(categoryATickets.Count == 1);
            Assert.True(categoryATickets.All(t => t.Category == "Updated Test Category"));
        }
    }

    [Fact, TestPriority(5)]
    public void TestTicketProgressIncrement()
    {
        using (ViewContext db = new())
        {
            // Create a ticket with initial progress
            Ticket? ticket = db.Tickets.FirstOrDefault(t => t.Title == "Updated Test Ticket");

            Assert.NotNull(ticket);

            // Increment the ticket progress
            ticket.Progress += 10;
            db.Tickets.Update(ticket);
            db.SaveChanges();

            // Validate that the ticket progress has been incremented
            Assert.True(db.Tickets.Any(t => t.Title == "Updated Test Ticket" && t.Progress == 11));
        }
    }


    [Fact, TestPriority(6)]
    public void TestTicketDeletion()
    {
        using (ViewContext db = new())
        {
            // Delete the ticket created in the previous test
            Ticket? ticket = db.Tickets.FirstOrDefault(t => t.Title == "Updated Test Ticket");
            if (ticket != null)
            {
                db.Tickets.Remove(ticket);
                db.SaveChanges();

            }
            Assert.False(db.Tickets.Any(t => t.Title == "Updated Test Ticket"));
        }
    }

    [Fact, TestPriority(7)]
    public void TestUserCreation()
    {
        using (UserContext db = new())
        {
            ApplicationUser user = new()
            {
                Name = "Test User",
                UserName = "test@example.com",
                Email = "test@example.com"
            };

            db.Users.Add(user);
            db.SaveChanges();

            Assert.True(db.Users.Any(u => u.UserName == "test@example.com"));
        }
    }

    [Fact, TestPriority(8)]
    public void TestUserRole()
    {
        using (UserContext db = new())
        {
            // Check if the user created in the previous test has the 'User' role
            ApplicationUser? user = db.Users.FirstOrDefault(u => u.UserName == "test@example.com");

            Assert.NotNull(user);
            // Shouldn't have any roles yet
            Assert.False(db.UserRoles.All(ur => ur.UserId == user.Id));
        }
    }

    [Fact, TestPriority(9)]
    public void TestUserUpdate()
    {
        // Check if the user can be updated (e.g. set EmailConfirmed to true)
        using (UserContext db = new())
        {
            // Update the user created in the previous test
            ApplicationUser? user = db.Users.FirstOrDefault(u => u.UserName == "test@example.com");
            if (user != null)
            {
                user.EmailConfirmed = true;

                db.Users.Update(user);
                db.SaveChanges();
            }
            Assert.True(db.Users.Any(u => u.EmailConfirmed == true));
        }
    }

    [Fact, TestPriority(10)]
    public void TestUserUpdateChangeName()
    {
        // Check if the user's name can be updated
        using (UserContext db = new())
        {
            // Update the user created in the previous test
            ApplicationUser? user = db.Users.FirstOrDefault(u => u.UserName == "test@example.com");
            if (user != null)
            {
                user.Name = "Test User Updated";

                db.Users.Update(user);
                db.SaveChanges();
            }
            Assert.True(db.Users.Any(u => u.Name == "Test User Updated"));
        }
    }

    [Fact, TestPriority(11)]
    public void TestUserDeletion()
    {
        using (UserContext db = new())
        {
            // Delete the user created in the previous test
            ApplicationUser? user = db.Users.FirstOrDefault(u => u.UserName == "test@example.com");
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            Assert.False(db.Users.Any(u => u.UserName == "test@example.com"));
        }
    }
}
