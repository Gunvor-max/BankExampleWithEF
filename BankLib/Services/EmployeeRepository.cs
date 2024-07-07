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
        public Employee Isloggedin { get; set; }
        public EmployeeRepository() 
        {
            _context = new EFContext();
        }

        public Employee Create(Employee theObject)
        {
            _context.BankExampleWithEfEmployees.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public Employee Delete(Employee theObject)
        {
            throw new NotImplementedException();
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
