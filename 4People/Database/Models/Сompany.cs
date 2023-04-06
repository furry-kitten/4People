using System;
using System.Collections.Generic;
using _4People.ViewModels;

namespace _4People.Database.Models
{
    public class Company : BaseDbModel
    {
        public string Name { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Address { get; set; } = null!;

        public ICollection<Subdivision>? Subdivisions { get; set; }
    }
}
