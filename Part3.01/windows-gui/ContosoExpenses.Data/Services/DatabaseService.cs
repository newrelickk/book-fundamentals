using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bogus;
using LiteDB;
using ContosoExpenses.Data.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace ContosoExpenses.Data.Services
{
    public class DatabaseService: IDatabaseService
    {
        private readonly HttpClient client;
        private string baseUrl;

        public DatabaseService()
        {
            client = new HttpClient();
            baseUrl = Environment.GetEnvironmentVariable("APIBASE_URL") ?? "http://localhost:5000";
        }

        public Employee GetEmployee(int employeeId)
        {
            async Task<Employee> GetAsync()
            {
                //var res = await client.GetAsync($"{baseUrl}employee/{employeeId}").ConfigureAwait(false);
                //var cont = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                //return JsonConvert.DeserializeObject<Employee>(cont);
                var employees = GetEmployees();
                await Task.Delay(TimeSpan.FromSeconds(1.5)).ConfigureAwait(false);
                return employees.FirstOrDefault(e => e.EmployeeId == employeeId);
            }
            return GetAsync().GetAwaiter().GetResult();
        }

        public List<Employee> GetEmployees()
        {
            async Task<Employee[]> GetAsync()
            {
                var res = await client.GetAsync($"{baseUrl}employee").ConfigureAwait(false);
                var cont = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Employee[]>(cont);
            }
            return GetAsync().GetAwaiter().GetResult().ToList();
        }

        public List<Expense> GetExpenses(int employeedId)
        {
            async Task<Expense[]> GetAsync()
            {
                var res = await client.GetAsync($"{baseUrl}expense/employee/{employeedId}").ConfigureAwait(false);
                var cont = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Expense[]>(cont);
            }
            return GetAsync().GetAwaiter().GetResult().ToList();
        }

        public Expense GetExpense(int expenseId)
        {
            async Task<Expense> GetAsync()
            {
                var res = await client.GetAsync($"{baseUrl}expense/{expenseId}").ConfigureAwait(false);
                var cont = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Expense>(cont);
            }
            return GetAsync().GetAwaiter().GetResult();
        }

        public void SaveExpense(Expense expense)
        {
            async Task Save()
            {
                try
                {
                    var res = await client.PostAsync($"{baseUrl}expense/", new StringContent(JsonConvert.SerializeObject(expense), Encoding.UTF8, "application/json")).ConfigureAwait(false);
                    res.EnsureSuccessStatusCode();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    //
                }
                
            }
            Save().GetAwaiter().GetResult();
        }


    }
}
