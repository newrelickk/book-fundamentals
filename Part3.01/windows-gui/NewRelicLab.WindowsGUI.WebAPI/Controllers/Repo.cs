using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewRelicLab.WindowsGUI.WebAPI.Controllers
{
    public static class Repo
    {
        private static readonly int numberOfEmployees = 50;
        private static readonly int numberOfExpenses = 10;
        internal readonly static Dictionary<int, Employee> employeeDictionary = new Dictionary<int, Employee>();
        internal readonly static Dictionary<int, Expense> expenseDictionary = new Dictionary<int, Expense>();
        internal readonly static Dictionary<int, List<int>> employeeToExpensesDictionary = new Dictionary<int, List<int>>();
        
        public static void Initialize()
        {
            var i = 0;
            for (int cont = 0; cont < numberOfEmployees; cont++)
            {
                var employee = new Faker<Employee>()
                    .RuleFor(x => x.FirstName, (f, u) => f.Name.FirstName())
                    .RuleFor(x => x.LastName, (f, u) => f.Name.LastName())
                    .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, "contoso.com"))
                    .Generate();

                employee.EmployeeId = cont;
                employeeDictionary.Add(cont, employee);

                var list = new List<int>();
                for (int contExpenses = 0; contExpenses < numberOfExpenses; contExpenses++)
                {
                    var expense = new Faker<Expense>()
                   .RuleFor(x => x.Description, (f, u) => f.Commerce.ProductName())
                   .RuleFor(x => x.Type, (f, u) => f.Finance.TransactionType())
                   .RuleFor(x => x.Cost, (f, u) => (double)f.Finance.Amount())
                   .RuleFor(x => x.Address, (f, u) => f.Address.FullAddress())
                   .RuleFor(x => x.City, (f, u) => f.Address.City())
                   .RuleFor(x => x.Date, (f, u) => f.Date.Past())
                   .Generate();

                    expense.EmployeeId = cont;
                    expense.ExpenseId = i;
                    expenseDictionary.Add(i, expense);
                    list.Add(i);
                    ++i;
                }
                employeeToExpensesDictionary.Add(cont, list);
            }
        }
    }
}
