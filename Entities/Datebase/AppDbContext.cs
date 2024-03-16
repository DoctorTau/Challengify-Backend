using Challengify.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

public class AppDbContext : DbContext
{
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Result> Results { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Env.Load(Path.Combine("..", ".env")); // Load the .env file at the application startup

        // Retrieve each component of the connection string from the environment variables
        var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");

        // Construct the connection string
        var connectionString = $"Host=localhost;Database={dbName};Username={dbUser};Password={dbPassword}";

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}