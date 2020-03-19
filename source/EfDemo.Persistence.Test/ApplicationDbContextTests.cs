using EfDemo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfDemo.Persistence.Test
{
    [TestClass]
    public class ApplicationDbContextTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            // Build the ApplicationDbContext 
            //  - with InMemory-DB
            return new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging()
                .Options);
        }

        [TestMethod]
        public async Task ApplicationDbContext_AddSchoolClass_ShouldPersistSchoolClass()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = new SchoolClass { Name = "6ABIF_6AKIF" };
                Assert.AreEqual(0, schoolClass.Id);
                dbContext.SchoolClasses.Add(schoolClass);
                await dbContext.SaveChangesAsync();
                Assert.AreNotEqual(0, schoolClass.Id);
            }

            using (ApplicationDbContext verifyContext = GetDbContext(dbName))
            {
                StringBuilder logText = new StringBuilder();
                Assert.AreEqual(1, await verifyContext.SchoolClasses.CountAsync());
                SchoolClass schoolClass = await verifyContext.SchoolClasses.FirstAsync();
                Assert.IsNotNull(schoolClass);
                Assert.AreEqual("6ABIF_6AKIF", schoolClass.Name);

            }
        }

        [TestMethod]
        public async Task ApplicationDbContext_AddSchoolClassWithPupils_QueryPupils_ShouldReturnPupils()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = new SchoolClass
                {
                    Name = "6ABIF_6AKIF",
                    Pupils = new List<Pupil>
                    {
                        new Pupil
                        {
                            FirstName = "Max",
                            LastName = "Mustermann",
                            BirthDate = DateTime.Parse("1.1.1990")
                        },
                        new Pupil
                        {
                            FirstName = "Eva",
                            LastName = "Musterfrau",
                            BirthDate = DateTime.Parse("1.1.1991")
                        },
                        new Pupil
                        {
                            FirstName = "Fritz",
                            LastName = "Musterkind",
                            BirthDate = DateTime.Parse("1.1.1980")
                        },
                        new Pupil
                        {
                            FirstName = "Franz", LastName = "Huber", BirthDate = DateTime.Parse("10.7.1999")
                        }
                    }
                };
                dbContext.SchoolClasses.Add(schoolClass);
                await dbContext.SaveChangesAsync();
                Assert.AreNotEqual(0, schoolClass.Id);
            }

            using (ApplicationDbContext queryContext = GetDbContext(dbName))
            {
                Assert.AreEqual(1, await queryContext.SchoolClasses.CountAsync());
                Assert.AreEqual(4, await queryContext.Pupils.CountAsync());
                // Ältester Schüler
                Pupil eldest = await queryContext.Pupils.OrderBy(pupil => pupil.BirthDate).FirstAsync();
                Assert.AreEqual("Musterkind", eldest.LastName);
            }
        }


        [TestMethod]
        public async Task ApplicationDbContext_UpdateSchoolClass_ShouldReturnChangedSchoolClass()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = new SchoolClass { Name = "5ABIF_5AKIF" };
                Assert.AreEqual(0, schoolClass.Id);
                dbContext.SchoolClasses.Add(schoolClass);
                await dbContext.SaveChangesAsync();
                Assert.AreNotEqual(0, schoolClass.Id);
            }

            using (ApplicationDbContext updateContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = await updateContext.SchoolClasses.FirstAsync();
                schoolClass.Name = "6ABIF_6AKIF";
                await updateContext.SaveChangesAsync();
            }

            using (ApplicationDbContext verifyContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = await verifyContext.SchoolClasses.FirstAsync();
                Assert.AreEqual("6ABIF_6AKIF", schoolClass.Name);
            }
        }

        [TestMethod]
        public async Task ApplicationDbContext_DeleteSchoolClass_ShouldReturnZeroSchoolClasses()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = new SchoolClass { Name = "6ABIF_6AKIF" };
                Assert.AreEqual(0, schoolClass.Id);
                dbContext.SchoolClasses.Add(schoolClass);
                dbContext.SchoolClasses.Add(new SchoolClass { Name = "5ABIF_5AKIF" });
                await dbContext.SaveChangesAsync();
                Assert.AreNotEqual(0, schoolClass.Id);
            }

            using (ApplicationDbContext deleteContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = await deleteContext.SchoolClasses.SingleAsync(sc => sc.Name == "5ABIF_5AKIF");
                deleteContext.SchoolClasses.Remove(schoolClass);
                deleteContext.SaveChanges();
            }

            using (ApplicationDbContext verifyContext = GetDbContext(dbName))
            {
                Assert.AreEqual(1, await verifyContext.SchoolClasses.CountAsync());
                var schoolClass = await verifyContext.SchoolClasses.FirstAsync();
                Assert.AreEqual("6ABIF_6AKIF", schoolClass.Name);
            }
        }

        [TestMethod]
        public void ApplicationDbContext_DeletePupilFromClass_ShouldReturnTwoPupils()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = new SchoolClass
                {
                    Name = "6ABIF_6AKIF",
                    Pupils = new List<Pupil>
                    {
                        new Pupil
                        {
                            FirstName = "Max",
                            LastName = "Mustermann",
                            BirthDate = DateTime.Parse("1.1.1990")
                        },
                        new Pupil
                        {
                            FirstName = "Eva",
                            LastName = "Musterfrau",
                            BirthDate = DateTime.Parse("1.1.1991")
                        },
                        new Pupil
                        {
                            FirstName = "Fritz",
                            LastName = "Musterkind",
                            BirthDate = DateTime.Parse("1.1.1980")
                        },
                        new Pupil
                        {
                            FirstName = "Franz", LastName = "Huber", BirthDate = DateTime.Parse("10.7.1999")
                        }
                    }
                };

                dbContext.SchoolClasses.Add(schoolClass);
                dbContext.SaveChanges();
                Assert.AreNotEqual(0, schoolClass.Id);
            }

            using (ApplicationDbContext deleteContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass = deleteContext.SchoolClasses
                    .Include(sc => sc.Pupils)
                    .Single(sc => sc.Name == "6ABIF_6AKIF");
                //var pupils = deleteContext.Pupils.ToList(); => durch include ersetzt
                Pupil pupil1 = schoolClass.Pupils.Single(p => p.LastName == "Musterkind");
                Pupil pupil2 = schoolClass.Pupils.Single(p => p.LastName == "Huber");

                deleteContext.Pupils.Remove(pupil1);
                deleteContext.Pupils.Remove(pupil2);

                deleteContext.SaveChanges();
            }

            using (ApplicationDbContext verifyContext = GetDbContext(dbName))
            {
                Assert.AreEqual(1, verifyContext.SchoolClasses.Count());
                Assert.AreEqual(2, verifyContext.Pupils.Count());
            }
        }

        [TestMethod]
        public void ApplicationDbContext_MovePupilToAnotherClass_ShouldReturnCorrectPupils()
        {
            string dbName = Guid.NewGuid().ToString();

            using (ApplicationDbContext dbContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass5 = new SchoolClass { Name = "5ABIF_5AKIF" };
                
                
                SchoolClass schoolClass6 = new SchoolClass
                {
                    Name = "6ABIF_6AKIF",
                    Pupils = new List<Pupil>
                    {
                        new Pupil
                        {
                            FirstName = "Max",
                            LastName = "Mustermann",
                            BirthDate = DateTime.Parse("1.1.1990")
                        },
                        new Pupil
                        {
                            FirstName = "Eva",
                            LastName = "Musterfrau",
                            BirthDate = DateTime.Parse("1.1.1991")
                        },
                        new Pupil
                        {
                            FirstName = "Fritz",
                            LastName = "Musterkind",
                            BirthDate = DateTime.Parse("1.1.1980")
                        },
                        new Pupil
                        {
                            FirstName = "Franz", LastName = "Huber", BirthDate = DateTime.Parse("10.7.1999")
                        }
                    }
                };

                dbContext.SchoolClasses.Add(schoolClass5);
                dbContext.SchoolClasses.Add(schoolClass6);
                dbContext.SaveChanges();
                Assert.AreNotEqual(0, schoolClass5.Id);
                Assert.AreNotEqual(0, schoolClass6.Id);
                Assert.AreNotEqual(schoolClass5.Id, schoolClass6.Id);
            }

            using (ApplicationDbContext moveContext = GetDbContext(dbName))
            {
                SchoolClass schoolClass5 = moveContext.SchoolClasses
                                                .Include(sc => sc.Pupils)
                                                .Single(sc => sc.Name == "5ABIF_5AKIF");
                SchoolClass schoolClass6 = moveContext.SchoolClasses
                                                .Include(sc => sc.Pupils)
                                                .Single(sc => sc.Name == "6ABIF_6AKIF");
                //var pupils = moveContext.Pupils.ToList(); => durch include ersetzt
                Pupil pupil = schoolClass6.Pupils.Single(p => p.LastName == "Musterkind");

                schoolClass5.Pupils.Add(pupil);

                moveContext.SaveChanges();
            }

            using (ApplicationDbContext verifyContext = GetDbContext(dbName))
            {
                Assert.AreEqual(2, verifyContext.SchoolClasses.Count());
                Assert.AreEqual(1, verifyContext.Pupils.Count(sc => sc.SchoolClass.Name == "5ABIF_5AKIF"));
                Assert.AreEqual(3, verifyContext.Pupils.Count(sc => sc.SchoolClass.Name == "6ABIF_6AKIF"));
            }
        }
    }
}
