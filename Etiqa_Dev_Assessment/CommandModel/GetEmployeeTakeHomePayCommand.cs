using Etiqa_Dev_Assessment.Models;

namespace Etiqa_Dev_Assessment.CommandModel
{
    public class GetEmployeeTakeHomePayCommand
    {
        public GetEmployeeTakeHomePayCommand() { }

        public string EmployeeNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
