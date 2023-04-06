using System.Collections.Generic;
using _4People.Database.Models;

namespace _4People.Models
{
    public class PayrollSheet
    {
        public Company Company { get; set; } = null!;
        public List<Employee> Employees { get; set; } = new();
        public List<Subdivision> Subdivisions { get; set; } = new();
        public Dictionary<Subdivision, decimal> SubdivisionsResults { get; set; } = new();
        public decimal Total { get; set; }
    }
}