namespace WebAPI.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

using WebAPI.Entities;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Tree> Trees { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Event> Events { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Tree>(b =>
        {
            b.HasMany(e => e.Children)
                    .WithOne(e => e.Tree)
                    .HasForeignKey(ur => ur.TreeId);
        });
    }
}