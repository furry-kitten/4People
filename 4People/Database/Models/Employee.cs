using System;

namespace _4People.Database.Models
{
    public class Employee : BaseDbModel
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public DateTime EmploymentDate { get; set; }
        public string Rank { get; set; } = null!;
        public decimal Salary { get; set; }
        public Guid SubdivisionId { get; set; }

        public Subdivision? Subdivision { get; set; }
        public Subdivision? SubordinateUnit { get; set; }
    }
}
