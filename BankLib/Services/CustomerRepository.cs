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
    public class CustomerRepository : ICustomerRepository
    {
        private EFContext _context;
        public string? LogText { get; set; }
        public Customer Isloggedin { get; set; }
        public CustomerRepository()
        {
            _context = new EFContext();
        }

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
            LogText = theObject.ToString();
            _context.BankExampleWithEfCustomers.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public Customer Delete(Customer theObject)
        {
            LogText = theObject.ToString();
            _context.BankExampleWithEfCustomers.Update(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public List<Customer> GetAll()
        {
            return _context.BankExampleWithEfCustomers
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .Where(e => e.IsDeleted == false)
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

                    if(originalValue.ToString() != currentValue.ToString())
                    {
                    // Process the information
                    LogText += ($"Class={entityType}, Property={propertyName}, Original={originalValue}, Current={currentValue}:");
                    }
                }
            }

            //Update object
            _context.BankExampleWithEfCustomers.Update(theObject);
            _context.SaveChanges();
            return theObject;
        }
    }
}
