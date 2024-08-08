using System;
using Main.Consts;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main;

public class Context : DbContext {
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<Account> Account { get; set; }
    public DbSet<CardCategory> CardCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardCategory>().HasOne(cc => cc.Account);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStrings.Default) ?? "";
        optionsBuilder.UseSqlServer(connectionString);
    }
}