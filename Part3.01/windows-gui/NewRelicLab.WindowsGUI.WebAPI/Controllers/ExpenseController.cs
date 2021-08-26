using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewRelicLab.WindowsGUI.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : ControllerBase
    {
        [HttpGet]
        [Route("employee/{id}")]
        public IEnumerable<Expense> GetByEmployee(int id)
        {
            return Repo.employeeToExpensesDictionary[id].Select(i => Repo.expenseDictionary[i]).ToArray();
        }

        [HttpGet("{id}")]
        public Expense Get(int id)
        {
            return Repo.expenseDictionary[id];
        }

        [HttpPost]
        public void Save(Expense expense)
        {
            lock(Repo.employeeDictionary)
            {
                int newId = Repo.expenseDictionary.Values.Max(e => e.ExpenseId) + 1;
                expense.ExpenseId = newId;
                Repo.employeeToExpensesDictionary[expense.EmployeeId].Add(expense.ExpenseId);
                Repo.expenseDictionary.Add(expense.ExpenseId, expense);
            }
        }
    }
}
