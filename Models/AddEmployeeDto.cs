using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace emp.Models
{
    public class AddEmployeeDto
    {
        public required string Name {get;set;}
        public required string Email {get;set;}
        public string? Phone {get;set;}
        public decimal Salary {get;set;}
    }
}