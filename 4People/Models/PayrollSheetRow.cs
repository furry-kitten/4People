namespace _4People.Models
{
    public class PayrollSheetRow
    {
        public string Company { get; set; } = null!;
        public string EmployeeFullName { get; set; } = null!;
        public string SubdivisionName { get; set; } = null!;
        public decimal Salary { get; set; }
    }
}