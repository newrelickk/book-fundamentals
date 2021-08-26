using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewRelicLab.WindowsGUI.WebAPI
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeId { get; set; }
    }
}
