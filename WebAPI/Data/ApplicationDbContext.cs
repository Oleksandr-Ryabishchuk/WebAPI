﻿namespace WebAPI.Data;

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
}