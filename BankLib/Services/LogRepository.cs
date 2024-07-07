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
    public class LogRepository : ILogRepository
    {
        EFContext _context;
        public LogRepository() 
        {
            _context = new EFContext();
        }

        public EmployeeLog Create(EmployeeLog theObject)
        {
            _context.BankExampleWithEfEmployeeLogs.Add(theObject);
            _context.SaveChanges();
            return theObject;
        }

        public EmployeeLog Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeLog> GetAll()
        {
            return _context.BankExampleWithEfEmployeeLogs.ToList();
        }

        public EmployeeLog? Read(int id)
        {
            return _context.BankExampleWithEfEmployeeLogs
                .Where(i => i.LogId == id).FirstOrDefault();
        }

        public EmployeeLog Update(EmployeeLog theObject, int id)
        {
            throw new NotImplementedException();
        }
    }
}
