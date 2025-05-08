using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>(platform =>
        {
            platform.HasKey(p => p.Id);

            platform.Property(p => p.Id)
                .ValueGeneratedNever()
                .IsRequired();

            platform.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            platform
                .HasMany(p => p.Commands)
                .WithOne(c => c.Platform)
                .HasForeignKey(c => c.PlatformId);
        });

        modelBuilder.Entity<Command>(command =>
        {
            command.HasKey(c => c.Id);

            command.Property(c => c.HowTo)
                .IsRequired()
                .HasMaxLength(2000);

            command.Property(c => c.CommandLine)
                .IsRequired()
                .HasMaxLength(1000);

            command
                .HasOne(c => c.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(c => c.PlatformId);
        });
    }
}