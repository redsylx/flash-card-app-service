using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main;

public class Context : DbContext {
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<Account> Account { get; set; }
}