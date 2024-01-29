using Microsoft.EntityFrameworkCore;
using View.Shared; // ViewContext, UserContext, Ticket

namespace View.UnitTests;

[TestCaseOrderer("View.UnitTests.PriorityOrderer", "View.UnitTests")]
public class DbConnectionTests
{
    [Fact, TestPriority(1)]
    public void TestTicketDatabaseCreation()
    {
        using (ViewContext db = new())
        {
            if (db.Database.CanConnect())
            {
                // DB already exists, so we can't create it again.
                Assert.False(db.Database.EnsureCreated());
            }
            else
            {
                Assert.True(db.Database.EnsureCreated());
            }
        }
    }

    [Fact, TestPriority(2)]
    public void TestUserDatabaseCreation()
    {
        using (UserContext db = new())
        {
            if (db.Database.CanConnect())
            {
                Assert.False(db.Database.EnsureCreated());
            }
            else
            {
                Assert.True(db.Database.EnsureCreated());
            }
        }
    }

    [Fact, TestPriority(3)]
    public void TestTicketDatabaseConnection()
    {
        using (ViewContext db = new())
        {
            // Create ticket db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact, TestPriority(4)]
    public void TestUserDatabaseConnection()
    {
        using (UserContext db = new())
        {
            // Create user db if it doesn't exist
            db.Database.EnsureCreated();

            Assert.True(db.Database.CanConnect());
        }
    }

    [Fact, TestPriority(5)]
    public void TestTicketDatabaseIntegrity()
    {
        using (ViewContext db = new())
        {
            // AutoSavepointsEnabled should be true
            Assert.True(db.Database.AutoSavepointsEnabled);
        }
    }

    [Fact, TestPriority(6)]
    public void TestUserDatabaseIntegrity()
    {
        using (UserContext db = new())
        {
            // AutoSavepointsEnabled should be true
            Assert.True(db.Database.AutoSavepointsEnabled);
        }
    }

    [Fact, TestPriority(7)]
    public void TestTicketDatabaseNoMigration()
    {
        using (ViewContext db = new())
        {
            // No migrations should exist yet
            Assert.False(db.Database.GetMigrations().Any());
        }
    }

    [Fact, TestPriority(8)]
    public void TestUserDatabaseNoMigration()
    {
        using (UserContext db = new())
        {
            // No migrations should exist yet
            Assert.False(db.Database.GetMigrations().Any());
        }
    }

    [Fact, TestPriority(9)]
    public void TestTicketNoPendingMigrations()
    {
        using (ViewContext db = new())
        {
            // No migrations should be pending
            Assert.False(db.Database.GetPendingMigrations().Any());
        }
    }

    [Fact, TestPriority(10)]
    public void TestUserNoPendingMigrations()
    {
        using (UserContext db = new())
        {
            // No migrations should be pending
            Assert.False(db.Database.GetPendingMigrations().Any());
        }
    }

    [Fact, TestPriority(11)]
    public void TestTicketDatabaseRelationalModel()
    {
        using (ViewContext db = new())
        {
            // Relational model is required
            Assert.True(db.Database.IsRelational());
        }
    }

    [Fact, TestPriority(12)]
    public void TestUserDatabaseRelationalModel()
    {
        using (UserContext db = new())
        {
            // Relational model is required
            Assert.True(db.Database.IsRelational());
        }
    }

    [Fact, TestPriority(13)]
    public void TestTicketDatabaseName()
    {
        using (ViewContext db = new())
        {
            // Proper database name is required for DataContext to work
            Assert.Equal("Tickets", db.Database.GetDbConnection().Database);
        }
    }

    [Fact, TestPriority(14)]
    public void TestUserDatabaseName()
    {
        using (UserContext db = new())
        {
            // Proper database name is required for UserDataContext to work
            Assert.Equal("Users", db.Database.GetDbConnection().Database);
        }
    }

    [Fact, TestPriority(15)]
    public void TestTIcketDatabaseTransactionBehavior()
    {
        using (ViewContext db = new())
        {
            Assert.True(db.Database.AutoTransactionBehavior == AutoTransactionBehavior.WhenNeeded);
        }
    }

    [Fact, TestPriority(16)]
    public void TestUserDatabaseTransactionBehavior()
    {
        using (UserContext db = new())
        {
            Assert.True(db.Database.AutoTransactionBehavior == AutoTransactionBehavior.WhenNeeded);
        }
    }

    [Fact, TestPriority(17)]
    public void TestTicketDatabaseTransactionManager()
    {
        using (ViewContext db = new())
        {
            // No transaction should be active
            Assert.True(db.Database.CurrentTransaction == null);
        }
    }

    [Fact, TestPriority(18)]
    public void TestUserDatabaseTransactionManager()
    {
        using (UserContext db = new())
        {
            // No transaction should be active
            Assert.True(db.Database.CurrentTransaction == null);
        }
    }
}
