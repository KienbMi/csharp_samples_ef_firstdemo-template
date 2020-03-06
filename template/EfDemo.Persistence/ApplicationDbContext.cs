using EfDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace EfDemo.Persistence
{
  public class ApplicationDbContext : DbContext
  {
    public DbSet<Pupil> Pupils { get; set; }

    public DbSet<SchoolClass> SchoolClasses { get; set; }

    public ApplicationDbContext() : base()
    {

    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        var configuration = new ConfigurationBuilder()
          .SetBasePath(Environment.CurrentDirectory)
          .AddJsonFile("appsettings.json")
          .Build();

        string connectionString = configuration["ConnectionStrings:DefaultConnection"];

        optionsBuilder.UseSqlServer(connectionString);
      }
    }
  }
}
