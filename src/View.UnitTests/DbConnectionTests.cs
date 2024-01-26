using Microsoft.EntityFrameworkCore;
using View.Shared; // ViewContext, UserContext, Ticket

namespace View.UnitTests;

[TestCaseOrderer("View.UnitTests.PriorityOrderer", "View.UnitTests")]
public class DbConnectionTests
{
    [Fact, TestPriority(1)]
    public void TestTicketDatabaseConnection()
    {
        using (ViewContext db = new())
        {
            // Create ticket db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact, TestPriority(2)]
    public void TestUserDatabaseConnection()
    {
        using (UserContext db = new())
        {
            // Create user db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact, TestPriority(3)]
    public void TestTicketDatabaseIntegrity()
    {
        using (ViewContext db = new())
        {
            Assert.True(db.Database.AutoSavepointsEnabled);
        }
    }

    [Fact, TestPriority(4)]
    public void TestUserDatabaseIntegrity()
    {
        using (UserContext db = new())
        {
            Assert.True(db.Database.AutoSavepointsEnabled);
        }
    }

    [Fact, TestPriority(5)]
    public void TestTicketDatabaseMigration()
    {
        using (ViewContext db = new())
        {
            // No migrations should be pending
            Assert.False(db.Database.GetMigrations().Any());
        }
    }

    [Fact, TestPriority(6)]
    public void TestUserDatabaseMigration()
    {
        using (UserContext db = new())
        {
            // No migrations should be pending
            Assert.False(db.Database.GetMigrations().Any());
        }
    }
}
