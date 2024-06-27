using BankLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services.Interfaces
{
    public interface IAccountRepository : IGenericCRUDInterface<Account>
    {
        List<Account> ReadAccountsConnectedToMain(int mainAccountId);
        public void Withdraw(int id, int amount);
        public void Deposit(int id, int amount);
    }
}
