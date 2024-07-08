using BankLib.Model;
using BankLib.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private EFContext _context;
        public string? LogText { get; set; }
        public Employee Isloggedin { get; set; }
        public EmployeeRepository() 
        {
            _context = new EFContext();
        }

        public Employee Create(Employee theObject)
        {
            _context.BankExampleWithEfEmployees.Add(theObject);
            _context.SaveChanges();
            LogText = theObject.ToString();
            return theObject;
        }

        public Employee Delete(Employee theObject)
        {
            _context.BankExampleWithEfEmployees.Update(theObject);
            _context.SaveChanges();
            LogText = theObject.ToString();
            return theObject;
        }

        public List<Employee> GetAll()
        {
            return _context.BankExampleWithEfEmployees
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.Position) // Include Position entity
        .Include(e => e.Department) // Include Department entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .Where(e => e.IsDeleted == false)
        .ToList();
        }

        public Employee? Read(int id)
        {
            return _context.BankExampleWithEfEmployees
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.Position) // Include Position entity
        .Include(e => e.Department) // Include Department entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .FirstOrDefault(e => e.EmployeeId == id); // Find employee by ID
        }

        public Employee? Update(Employee theObject, int id)
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

            _context.BankExampleWithEfEmployees.Update(theObject);
            _context.SaveChanges();
            return theObject;
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

        public List<Employee>? Search(string search)
        {
            return _context.BankExampleWithEfEmployees
        .Include(e => e.Address) // Include Address entity
        .Include(e => e.Address.City) // Include City entity
        .Include(e => e.Address.City.ZipCode) // Include Zipcode entity
        .Include(e => e.Position) // Include Position entity
        .Include(e => e.Department) // Include Department entity
        .Include(e => e.MainAccount) // Include MainAccount entity;
        .Where(e => e.IsDeleted == false && EF.Functions.Like(e.FirstName, $"%{search}%")).ToList(); // Find employee by ID
        }
    }
}
