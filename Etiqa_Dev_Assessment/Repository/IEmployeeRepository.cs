using Etiqa_Dev_Assessment.CommandModel;
using Etiqa_Dev_Assessment.Models;

namespace Etiqa_Dev_Assessment.Repository
{
    public interface IEmployeeRepository<T> where T : class
    {
        Task<Result> CreateEmployee(EmployeeCommand cmd);
        Task<Result> GetEmployeeTakeHomePay(GetEmployeeTakeHomePayCommand cmd);
        Task<Result> GetAllEmployees(int? id);
        Task<Result> DeleteEmployee(int id);
    }
}
