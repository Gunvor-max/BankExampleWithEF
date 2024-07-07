﻿using BankLib.Model;
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
            return theObject;
        }

        public Account Delete(Account theObject)
        {
            LogText = theObject.ToString();
            _context.BankExampleWithEfAccounts.Update(theObject);
            _context.SaveChanges();
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
