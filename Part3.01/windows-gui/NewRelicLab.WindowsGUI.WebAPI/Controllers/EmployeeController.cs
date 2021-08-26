using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewRelicLab.WindowsGUI.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return Repo.employeeDictionary.Values.ToArray();
        }

        [HttpGet("{id}")]
        public Employee Get(int id)
        {
            return Repo.employeeDictionary[id];
        }
    }
}
