using BankLib.Model;
using BankLib.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services
{
    public class TransactionRepository :ITransactionRepository
    {
        private EFContext _context;
        public TransactionRepository()
        {
            _context = new EFContext();
        }

        public Transaction Create(Transaction theObject)
        {
            _context.BankExampleWithEfTransactions.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public Transaction Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> GetAll()
        {
            return _context.BankExampleWithEfTransactions.Include(eid => eid.EmployeeId).Include(cid => cid.CustomerId).ToList();
        }

        public Transaction Read(int id)
        {
            return _context.BankExampleWithEfTransactions.Include(eid => eid.EmployeeId).Include(cid => cid.CustomerId).FirstOrDefault(i => i.TransactionId == id);
        }

        public List<Transaction> ReadAccountTransactions(int id)
        {
            return _context.BankExampleWithEfTransactions
                .Include(a => a.Account)
                .Include(c => c.Customer)
                .Where(i => i.AccountId == id)
                .ToList();
        }

        public Transaction Update(Transaction theObject, int id)
        {
            throw new NotImplementedException();
        }
    }
}
