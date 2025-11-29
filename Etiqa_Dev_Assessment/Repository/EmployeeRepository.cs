
using AppContext.Data;
using Dapper;
using Etiqa_Dev_Assessment.CommandModel;
using Etiqa_Dev_Assessment.Model;
using Etiqa_Dev_Assessment.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Etiqa_Dev_Assessment.Repository
{
    public class EmployeeRepository : IEmployeeRepository<Employee>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeRepository(AppDbContext context, IConfiguration configuration) : base()
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result> CreateEmployee(EmployeeCommand cmd)
        {
            var result = new Result();
            try
            {
                await ValidateEmployee(cmd.FirstName, cmd.LastName, cmd.MiddleName,
                    cmd.DateOfBirth, cmd.DailyRate, cmd.WorkingDays).ConfigureAwait(false);

                var random = new Random();
                string empNum = random.Next(0, 99999).ToString("D5");
                string codeName = cmd.LastName.Substring(0, Math.Min(3, cmd.LastName.Length)).ToUpper();

                string connString = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connString);
                await conn.OpenAsync();


                var insertedEmployee = await conn.QueryFirstOrDefaultAsync<Employee>(
                    "create_employee",
                    new
                    {
                        EmployeeNumber = $"{codeName}-{empNum}-{cmd.DateOfBirth.ToString("ddMMyyyy")}",
                        EmployeeName = cmd.MiddleName == null
                        ? $"{cmd.FirstName} {cmd.LastName}"
                        : $"{cmd.FirstName} {cmd.MiddleName} {cmd.LastName}",
                        DateOfBirth = cmd.DateOfBirth,
                        DailyRate = cmd.DailyRate,
                        WorkingDays = cmd.WorkingDays
                    },
                    commandType: CommandType.StoredProcedure
                );

                result.Success("Employee created successfully");

                return result;
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> ValidateEmployee(string firstName, string lastName,
            string? middleName, DateTime dateOfBirth, int dailyRate, string workingDays)
        {
            var result = new Result();
            try
            {
                if (firstName.IsNullOrEmpty())
                {
                    return result.Exception("First name is required");
                }
                if (lastName.IsNullOrEmpty())
                {
                    return result.Exception("Last name is required");
                }
                if (dateOfBirth == null)
                {
                    return result.Exception("Date of birth is required");
                }
                if (dailyRate <= 0)
                {
                    return result.Exception("Daily rate must be greater than zero");
                }
                if (workingDays.IsNullOrEmpty())
                {
                    return result.Exception("Working days is required");
                }

                return result;
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> DeleteEmployee(int id)
        {
            var result = new Result();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connString);
                await conn.OpenAsync();

                if (id == 0) {
                    return result.Exception("Employee ID is required");
                }

                var employee = await conn.QueryFirstOrDefaultAsync<Employee>(
                        "get_employee_by_id",
                        new { Id = id },
                        commandType: CommandType.StoredProcedure
                    );

                if (employee == null)
                    return result.Exception("Employee not found");

                await conn.ExecuteAsync(
                    "DeleteEmployee",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                );

                return result.Success("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
        public async Task<Result> UpdateEmployee(UpdateEmployeeCommand cmd)
        {
            var result = new Result();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connString);
                await conn.OpenAsync();

                var random = new Random();
                string empNum = random.Next(0, 99999).ToString("D5");
                string codeName = cmd.LastName.Substring(0, Math.Min(3, cmd.LastName.Length)).ToUpper();

                if (cmd.Id == 0)
                {
                    return result.Exception("Employee ID is required");
                }

                await conn.ExecuteAsync(
                    "UpdateEmployee",
                    new
                    {
                        Id = cmd.Id,
                        EmployeeNumber = $"{codeName}-{empNum}-{cmd.DateOfBirth.ToString("ddMMyyyy")}",
                        EmployeeName = cmd.MiddleName == null
                        ? $"{cmd.FirstName} {cmd.LastName}"
                        : $"{cmd.FirstName} {cmd.MiddleName} {cmd.LastName}",
                        DateOfBirth = cmd.DateOfBirth,
                        DailyRate = cmd.DailyRate,
                        WorkingDays = cmd.WorkingDays
                    },
                    commandType: CommandType.StoredProcedure
                );

                result.Success("Employee updated successfully");
                return result;
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }

        }

        public async Task<Result> GetAllEmployees(int? id)
        {
            var result = new Result();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connString);
                await conn.OpenAsync();

                if (id != null)
                {
                    var employee = await conn.QueryFirstOrDefaultAsync<Employee>(
                        "get_employee_by_id", 
                        new { Id = id },
                        commandType: CommandType.StoredProcedure
                    );

                    if (employee == null)
                        return result.Exception("Employee not found");

                    return result.Success("Employee retrieved successfully", employee);
                }
                else
                {
                    var employees = await conn.QueryAsync<Employee>(
                        "GetAllEmployee", 
                        commandType: CommandType.StoredProcedure
                    );

                    return result.Success("Employees retrieved successfully", employees);
                }
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }

        public async Task<Result> GetEmployeeTakeHomePay(GetEmployeeTakeHomePayCommand cmd)
        {
            var result = new Result();
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeNumber == cmd.EmployeeNumber);

                if (employee == null)
                    return result.Exception("Employee not found");

                var dayMap = new Dictionary<string, DayOfWeek>
                {
                    { "M", DayOfWeek.Monday },
                    { "T", DayOfWeek.Tuesday },
                    { "W", DayOfWeek.Wednesday },
                    { "TH", DayOfWeek.Thursday },
                    { "F", DayOfWeek.Friday },
                    { "S", DayOfWeek.Saturday },
                    { "U", DayOfWeek.Sunday }
                };
                string workingDaysStr = employee.WorkingDays.ToUpper();
                var workingDays = new List<DayOfWeek>();
                int i = 0;

                while (i < workingDaysStr.Length)
                {
                    if (i + 1 < workingDaysStr.Length && workingDaysStr.Substring(i, 2) == "TH")
                    {
                        workingDays.Add(dayMap["TH"]);
                        i += 2; 
                    }
                    else
                    {
                        string key = workingDaysStr[i].ToString();
                        workingDays.Add(dayMap[key]);
                        i += 1;
                    }
                }

                double totalPay = 0;
                var current = cmd.StartDate;

                while (current <= cmd.EndDate)
                {
                    if (workingDays.Contains(current.DayOfWeek))
                        totalPay += employee.DailyRate * 2;

                    if (current.Month == employee.DateOfBirth.Month && current.Day == employee.DateOfBirth.Day)
                        totalPay += employee.DailyRate;

                    current = current.AddDays(1);
                }

                var response = new
                {
                    EmployeeNumber = employee.EmployeeNumber,
                    EmployeeName = employee.EmployeeName,
                    TakeHomePay = totalPay,
                    StartingDate = cmd.StartDate.ToString("mm-dd-yyyy"),
                    EndingDate = cmd.EndDate.ToString("mm-dd-yyyy")
                };

                return result.Success("Calcualted Take Home Pay Successfully", response);
            }
            catch (Exception ex)
            {
                return result.Exception(ex.Message);
            }
        }
    }
}
