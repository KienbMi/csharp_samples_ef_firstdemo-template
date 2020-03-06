using EfDemo.Core.Entities;
using EfDemo.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfDemo.UI
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine("************************");
      Console.WriteLine("        EF-Demo");
      Console.WriteLine("************************");
      Console.WriteLine();

      await ResetDatabase();
      await CreateData();
      await PrintData();
    }

    private static async Task ResetDatabase()
    {
      using (ApplicationDbContext ctx = new ApplicationDbContext())
      {
        Console.WriteLine("Deleting the database (when it exists)...");
        await ctx.Database.EnsureDeletedAsync();
        Console.WriteLine("  > DONE");

        Console.WriteLine("Creating the database ...");
        await ctx.Database.EnsureCreatedAsync();
        Console.WriteLine("  > DONE");
        Console.WriteLine();
      }
    }


    private static async Task PrintData()
    {
      using (ApplicationDbContext ctx = new ApplicationDbContext())
      {
        Console.WriteLine($"Count of SchoolClasses: {ctx.SchoolClasses.Count()}");
        Console.WriteLine(
        $"Count of Pupils: {await ctx.Pupils.CountAsync()}");
        Console.WriteLine();

        foreach (var schoolClass in ctx.SchoolClasses)
        {
          Console.WriteLine($" > SchoolClass: '{schoolClass.Name}'");
        }

        Console.WriteLine("------------------------");

        foreach (var pupil in ctx.Pupils)
        {
          Console.WriteLine($" > Pupil: {pupil.FirstName} {pupil.LastName}");
        }
      }
    }

    private static async Task CreateData()
    {
      using (ApplicationDbContext ctx = new ApplicationDbContext())
      {
        SchoolClass schoolClass = new SchoolClass
        {
          Name = "6ABIF_6AKIF",
          Pupils = new List<Pupil>
                    {
                        new Pupil {FirstName = "Susi", LastName = "Mustermann",},
                        new Pupil {FirstName = "Maxi", LastName = "Mustermann",}
                    }
        };

        ctx.SchoolClasses.Add(schoolClass);
        await ctx.SaveChangesAsync();
      }
    }
  }
}