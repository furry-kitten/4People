namespace _4People.Models
{
    public class EmployeeInfo
    {
        public string CompanyName { get; set; } = null!;
        public string SubdivisionName { get; set; } = null!;
        public string FulName { get; set; } = null!;
        public int YearsInCompany { get; set; }
        public int Age { get; set; }
    }
}