using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankLib.Model;

namespace BankLib.Services.Interfaces
{
    public interface IEmployeeRepository : IGenericCRUDInterface<Employee>
    {
        bool CheckUser(string email, string password);
        Employee Isloggedin {  get; }
    }
}
