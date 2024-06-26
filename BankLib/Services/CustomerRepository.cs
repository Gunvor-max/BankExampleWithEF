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
    public class CustomerRepository : ICustomerRepository
    {
        private EFContext _context;
        public CustomerRepository()
        {
            _context = new EFContext();
        }
        public Customer Isloggedin { get; set; }

        public bool CheckUser(string email, string password)
        {
            Isloggedin = GetAll().Find(x => x.Mail == email && x.Password == password);
            if (Isloggedin != null)
            {
                return true;
            }
            return false;
        }

        public Customer Create(Customer theObject)
        {
            _context.BankExampleWithEfCustomers.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public Customer Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Customer> GetAll()
        {
            return _context.BankExampleWithEfCustomers
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .ToList();
        }

        public Customer? Read(int id)
        {
            return _context.BankExampleWithEfCustomers
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .FirstOrDefault(e => e.CustomerId == id); // Find employee by ID
        }

        public List<Customer> Search(string search)
        {
            return _context.BankExampleWithEfCustomers
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .Where(e => EF.Functions.Like(e.FirstName, $"%{search}%")).ToList(); // Find employee by ID
        }

        public Customer Update(Customer theObject, int id)
        {
            _context.BankExampleWithEfCustomers.Update(theObject);
            _context.SaveChanges();
            return theObject;
        }
    }
}
