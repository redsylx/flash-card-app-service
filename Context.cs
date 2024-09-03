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
    public DbSet<Card> Card { get; set; }
    // public DbSet<CardVersion> CardVersion { get; set; }
    public DbSet<Game> Game { get; set; }
    public DbSet<GameDetail> GameDetail { get; set; }
    public DbSet<GameDetailCategory> GameDetailCategory { get; set; }
    public DbSet<PointActivity> PointActivity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardCategory>().HasOne(p => p.Account);
        modelBuilder.Entity<Card>().HasOne(p => p.CardCategory);
        // modelBuilder.Entity<CardVersion>().HasOne(p => p.Card);
        modelBuilder.Entity<Game>().HasOne(p => p.Account);
        modelBuilder.Entity<Game>().HasMany(p => p.Categories);
        modelBuilder.Entity<Game>().HasMany(p => p.Details);
        modelBuilder.Entity<GameDetail>().HasOne(p => p.Game);
        // modelBuilder.Entity<GameDetail>().HasOne(p => p.CardVersion);
        modelBuilder.Entity<GameDetailCategory>().HasOne(p => p.Game);
        // modelBuilder.Entity<GameDetailCategory>().HasOne(p => p.CardCategory);
        modelBuilder.Entity<PointActivity>().HasOne(p => p.Account);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStrings.Default) ?? "";
        optionsBuilder.UseSqlServer(connectionString);
    }
}