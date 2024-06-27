using BankLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services.Interfaces
{
    public interface ITransactionRepository : IGenericCRUDInterface<Transaction>
    {
        public List<Transaction> ReadAccountTransactions(int id);
    }
}
