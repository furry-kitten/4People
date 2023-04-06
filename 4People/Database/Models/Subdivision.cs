using System;
using System.Collections.Generic;

namespace _4People.Database.Models
{
    public class Subdivision : BaseDbModel
    {
        public string Name { get; set; } = null!;
        public Guid? LeaderId { get; set; }
        public Guid CompanyId { get; set; }

        public Employee? Leader { get; set; }
        public Company? Company { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
