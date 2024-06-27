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
    public class AccountRepository : IAccountRepository
    {
        private EFContext _context;
        public AccountRepository() 
        {
            _context = new EFContext();
        }

        public Account Create(Account theObject)
        {
            _context.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public Account Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Account> GetAll()
        {
            return _context.BankExampleWithEfAccounts.Include(a => a.MainAccount).ToList();
        }

        public Account Read(int id)
        {
            return _context.BankExampleWithEfAccounts
            .FirstOrDefault(e => e.AccountId == id); // Find Account by ID
        }

        public List<Account> ReadAccountsConnectedToMain(int mainAccountId)
        {
            return _context.BankExampleWithEfAccounts
                .Where(a => a.MainAccountId == mainAccountId)
                .ToList();
        }

        public Account Update(Account theObject, int id)
        {
            throw new NotImplementedException();
        }

        public void Withdraw (int id, int amount)
        {
            Account account = Read(id);
            account.Balance -= amount;
            _context.SaveChanges();
        }

        public void Deposit(int id, int amount)
        {
            Account account = Read(id);
            account.Balance += amount;
            _context.SaveChanges();
        }
    }
}
