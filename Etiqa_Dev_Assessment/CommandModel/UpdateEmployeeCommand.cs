using Etiqa_Dev_Assessment.Models;

namespace Etiqa_Dev_Assessment.CommandModel
{
    public class UpdateEmployeeCommand : EmployeeCommand
    {
        public UpdateEmployeeCommand() { }

        public int Id { get; set; }
    }
}
