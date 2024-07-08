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
        public string LogText { get; set; }
        public AccountRepository() 
        {
            _context = new EFContext();
        }

        public Account Create(Account theObject)
        {
            _context.BankExampleWithEfAccounts.Add(theObject);
            _context.SaveChanges();
            LogText = theObject.ToString();
            return theObject;
        }

        public Account Delete(Account theObject)
        {
            _context.BankExampleWithEfAccounts.Update(theObject);
            _context.SaveChanges();
            LogText = theObject.ToString();
            return theObject;
        }

        public List<Account> GetAll()
        {
            return _context.BankExampleWithEfAccounts
                .Include(a => a.MainAccount)
                .Where(i => i.IsDeleted == false)
                .ToList();
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
            //Generate log
            LogText = string.Empty;
            var changes = _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            foreach (var change in changes)
            {
                // Get the entity type
                var entityType = change.Entity.GetType().Name;

                foreach (var property in change.CurrentValues.Properties)
                {
                    // Get the property name
                    var propertyName = property.Name;

                    // Get the current value
                    var currentValue = change.CurrentValues[propertyName];

                    // Get the original value (if available)
                    var originalValue = change.OriginalValues[propertyName];

                    if (originalValue.ToString() != currentValue.ToString())
                    {
                        // Process the information
                        LogText += $"Class={entityType},Property={propertyName},Original={originalValue},Current={currentValue}:";
                    }
                }
            }

            _context.BankExampleWithEfAccounts.Update(theObject);
            _context.SaveChanges();
            return theObject;
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

        public List<Account> Search(string search, int mainAccountID)
        {
            if (search is not null)
            {
                return ReadAccountsConnectedToMain(mainAccountID)
            .Where(e => e.Name.ToLower().Contains(search.ToLower())).ToList(); // Find account from name
            }
            else 
            {
                return ReadAccountsConnectedToMain(mainAccountID);
            }
        }

        public List<Account> Search(string search)
        {
            if (search is not null)
            {
                var result = GetAll()
            .Where(e => e.Name.ToLower().Contains(search.ToLower())).ToList();
                if (result.Count != 0)
                {
                    return result;
                }
                return GetAll()
            .Where(e => e.Type.ToLower().Contains(search.ToLower())).ToList(); // Find account from name
            }
            else
            {
                return GetAll();
            }
        }
    }
}
