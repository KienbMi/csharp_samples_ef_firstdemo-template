using System;

namespace EfDemo.Core.Entities
{
    public class Pupil
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        public SchoolClass SchoolClass { get; set; }

        public override string ToString() => $"Id: {Id}, FirstName: {FirstName}, LastName: {LastName}, BirthDate: {BirthDate.ToShortDateString()}, SchoolClass: {SchoolClass?.Name}";
    }
}
