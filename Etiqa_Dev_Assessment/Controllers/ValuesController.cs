using Etiqa_Dev_Assessment.CommandModel;
using Etiqa_Dev_Assessment.Model;
using Etiqa_Dev_Assessment.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Etiqa_Dev_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IEmployeeRepository<Employee> _repo;

        public ValuesController(IEmployeeRepository<Employee> repo)
        {
            _repo = repo;
        }

        // GET api/<ValuesController>/5
        [HttpGet("GetAllEmployee")]
        public async Task<object> GetAll([FromQuery]int? id)
        {
            var result = await _repo.GetAllEmployees(id).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            return json;
        }
        [HttpGet("GetEmployeeTakeHomePay")]
        public async Task<string> GetEmployeeTakeHomePay([FromQuery]GetEmployeeTakeHomePayCommand cmd)
        {
            var result = await _repo.GetEmployeeTakeHomePay(cmd).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            return json;
        }

        // POST api/<ValuesController>
        [HttpPost("CreateEmployee")]
        public async Task<string> Create([FromBody] EmployeeCommand cmd)
        {
            var result = await _repo.CreateEmployee(cmd).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            return json;
        }

        // PUT api/<ValuesController>/5
        [HttpPut("UpdateEmployee")]
        public async Task<string> Put([FromBody] UpdateEmployeeCommand cmd)
        {
            var result = await _repo.UpdateEmployee(cmd).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            return json;
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("DeleteEmployee")]
        public async Task<string> Delete([FromQuery]int id)
        {
            var result = await _repo.DeleteEmployee(id).ConfigureAwait(false);
            var json = JsonConvert.SerializeObject(result);
            return json;
        }
    }
}
