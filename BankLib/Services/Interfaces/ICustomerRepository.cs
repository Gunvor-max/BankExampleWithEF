using BankLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services.Interfaces
{
    public interface ICustomerRepository : IGenericCRUDInterface<Customer>
    {
        bool CheckUser(string email, string password);
        Customer Isloggedin { get; }
        List<Customer> Search(string search);
    }
}
