using System.Collections.Generic;

namespace EfDemo.Core.Entities
{
    public class SchoolClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Pupil> Pupils { get; set; } = new List<Pupil>();

        public override string ToString() => $"Id: {Id}, Name: {Name}, Pupils.Count: {Pupils.Count}";
    }
}
