using Etiqa_Dev_Assessment.Models;

namespace Etiqa_Dev_Assessment.CommandModel
{
    public class EmployeeCommand
    {
        public EmployeeCommand() { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int DailyRate { get; set; }
        public string WorkingDays { get; set; }
    }
}
