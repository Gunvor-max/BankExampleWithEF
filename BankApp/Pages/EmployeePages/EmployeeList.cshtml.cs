using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankLib.Model;
using System.Net;
using System.Reflection;
using BankLib.Services;

namespace BankApp.Pages.EmployeePages
{
    public class EmployeeListModel : PageModel
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogRepository _logRepository;
        private readonly IAccountRepository _accountRepository;
        public EmployeeListModel(IEmployeeRepository employeeRepository, ILogRepository logRepository, IAccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _logRepository = logRepository;
            _accountRepository = accountRepository;
            Employees = _employeeRepository.GetAll();
        }
        public List<Employee> Employees { get; set; }
        public static Employee? EmployeeLoggedIn {  get; set; }
        public string? AccountLogText { get; set; }

        [BindProperty]
        public string? Search { get; set; }

        //Employeebind
        [BindProperty]
        public int EmployeeId { get; set; }
        [BindProperty]
        public string Choosen_FirstName { get; set; }
        [BindProperty]
        public string Choosen_LastName { get; set; }
        [BindProperty]
        public string Choosen_Mail { get; set; }
        [BindProperty]
        public string Choosen_PhoneNumber { get; set; }
        [BindProperty]
        public string Choosen_Password { get; set; }
        [BindProperty]
        public string Choosen_Gender { get; set; }
        [BindProperty]
        public string Choosen_ImagePath { get; set; }

        //Addressbind
        [BindProperty]
        public string Choosen_StreetName { get; set; }
        [BindProperty]
        public int Choosen_HouseNumber { get; set; }
        [BindProperty]
        public string Choosen_City { get; set; }
        [BindProperty]
        public int Choosen_ZipCode { get; set; }

        //Positionbind
        [BindProperty]
        public string Choosen_Title { get; set; }
        [BindProperty]
        public string Choosen_PositionDescription { get; set; }
        [BindProperty]
        public int Choosen_AccessLevel { get; set;}

        //Departmentbind
        [BindProperty]
        public string Choosen_DepartmentName { get; set; }
        [BindProperty]
        public string Choosen_DepartmentDescription { get; set; }

        public bool IsEditMode { get; set; } = false;
        public bool IsDeleteable { get; set; } = false;
        public bool IsDeletedConfirmation { get; set; } = false;
        public Employee ShowEmployee { get; set; }
        public bool IsCustomerAgent { get; set; }
        public bool IsEditTriggered { get; set; } = false;
        public bool IsCreatedConfirmation { get; set; } = false;
        public bool IsUpdatedConfirmation { get; set; } = false;
        public bool CanSetPassword { get; set; } = false;
        public bool IsChoosen { get; set; } = false;
        public bool IsChoosenToCreate { get; set; } = false;
        public void OnGet()
        {
            //Check if the user has the proper accesslevel to view the page
            object user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                EmployeeLoggedIn = SessionHelper.Get<Employee>(EmployeeLoggedIn, HttpContext);
                if (EmployeeLoggedIn.Position.AccessLevel < 4)
                {
                    Response.Redirect("/LoginPages/Login");
                }
                Employees = _employeeRepository.GetAll();
            }
            else
            {
                Response.Redirect("/LoginPages/Login");
            }
        }

        public void OnPostSearchEmployees()
        {
            Employees = _employeeRepository.Search(Search);
        }

        public void OnPostSelectEmployee(int Id)
        {
            Employee employee = _employeeRepository.Read(Id);

            if (Id != 0)
            {
                if (employee is not null)
                {
                    //Employee
                    EmployeeId = employee.EmployeeId;
                    Choosen_FirstName = employee.FirstName;
                    Choosen_LastName = employee.LastName;
                    Choosen_Mail = employee.Mail;
                    Choosen_PhoneNumber = employee.PhoneNumber;
                    Choosen_Gender = employee.Gender;
                    Choosen_Password = employee.Password;
                    Choosen_ImagePath = employee.ImagePath;
                    //Address
                    Choosen_StreetName = employee.Address.StreetName;
                    Choosen_HouseNumber = employee.Address.HouseNumber;
                    Choosen_City = employee.Address.City.CityName;
                    Choosen_ZipCode = employee.Address.City.ZipCode.ZipCode;
                    //Position
                    Choosen_Title = employee.Position.Title;
                    Choosen_PositionDescription = employee.Position.Description;
                    Choosen_AccessLevel = employee.Position.AccessLevel;
                    //Department
                    Choosen_DepartmentName = employee.Department.Name;
                    Choosen_DepartmentDescription = employee.Department.Description;
                    
                }
            }
            IsEditMode = false;
            IsDeleteable = true;
            IsChoosen = true;
        }

        public void OnPostEnableEdit(int Id)
        {

                Employee employee = _employeeRepository.Read(Id);

                if (Id != 0)
                {
                    if (employee is not null)
                    {
                    //Employee
                    EmployeeId = employee.EmployeeId;
                    Choosen_FirstName = employee.FirstName;
                    Choosen_LastName = employee.LastName;
                    Choosen_Mail = employee.Mail;
                    Choosen_PhoneNumber = employee.PhoneNumber;
                    Choosen_Gender = employee.Gender;
                    Choosen_Password = employee.Password;
                    Choosen_ImagePath = employee.ImagePath;
                    //Address
                    Choosen_StreetName = employee.Address.StreetName;
                    Choosen_HouseNumber = employee.Address.HouseNumber;
                    Choosen_City = employee.Address.City.CityName;
                    Choosen_ZipCode = employee.Address.City.ZipCode.ZipCode;
                    //Position
                    Choosen_Title = employee.Position.Title;
                    Choosen_PositionDescription = employee.Position.Description;
                    Choosen_AccessLevel = employee.Position.AccessLevel;
                    //Department
                    Choosen_DepartmentName = employee.Department.Name;
                    Choosen_DepartmentDescription = employee.Department.Description;
                }
                }

            IsEditMode = true;
            IsEditTriggered = true;
            IsChoosen = true;
        }

        public void OnPostDeleteEmployee(int id)
        {
            Employee employee = _employeeRepository.Read(id);
            List<Account> accounts = _accountRepository.ReadAccountsConnectedToMain(employee.MainAccountId);
            if (id != 0)
            {
                if (employee is not null)
                {
                    employee.IsDeleted = true;
                    ShowEmployee = _employeeRepository.Delete(employee);

                    //Delete Accounts connected to employee
                    foreach (Account account in accounts)
                    {
                        account.IsDeleted = true;
                        _accountRepository.Delete(account);
                        AccountLogText += _accountRepository.LogText;
                    }

                    if (accounts.Count != 0)
                    {
                        //Create log for customer and accounts
                        EmployeeLog Log = new EmployeeLog
                        {
                            ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                            Date = DateTime.Now,
                            Activity = _employeeRepository.LogText + AccountLogText,
                            Type = "Employee and Accounts Deleted",
                            AffectedEmployeeId = employee.EmployeeId,
                        };

                        _logRepository.Create(Log);
                    }
                    else
                    {
                        //Create log for customer and accounts
                        EmployeeLog Log = new EmployeeLog
                        {
                            ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                            Date = DateTime.Now,
                            Activity = _employeeRepository.LogText,
                            Type = "Employee Deleted",
                            AffectedEmployeeId = employee.EmployeeId,
                        };

                        _logRepository.Create(Log);
                    }

                    Employees = _employeeRepository.GetAll();
                }
            }
            IsDeletedConfirmation = true;
        }

        public void OnPostUpdateEmployee(int id)
        {
                Employee employee = _employeeRepository.Read(id);

                if (id != 0)
                {
                    if (employee is not null)
                    {
                        employee.FirstName = Choosen_FirstName;
                        employee.LastName = Choosen_LastName;
                        employee.Mail = Choosen_Mail;
                        employee.PhoneNumber = Choosen_PhoneNumber;
                        employee.Gender = Choosen_Gender;
                        employee.ImagePath = Choosen_ImagePath;
                        employee.Address.StreetName = Choosen_StreetName;
                        employee.Address.HouseNumber = Choosen_HouseNumber;
                        employee.Address.City.CityName = Choosen_City;
                        employee.Address.City.ZipCode.ZipCode = Choosen_ZipCode;
                        employee.Position.Title = Choosen_Title;
                        employee.Position.Description = Choosen_PositionDescription;
                        employee.Position.AccessLevel = Choosen_AccessLevel;
                        employee.Department.Name = Choosen_DepartmentName;
                        employee.Department.Description = Choosen_DepartmentDescription;
                        ShowEmployee = _employeeRepository.Update(employee, id);
                        Employees = _employeeRepository.GetAll();

                        //Create log
                        EmployeeLog Log = new EmployeeLog
                        {
                        ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                        Date = DateTime.Now,
                        Activity = _employeeRepository.LogText,
                        Type = "Employee Updated",
                        AffectedEmployeeId = employee.EmployeeId,
                        };
                        _logRepository.Create(Log);
                        IsUpdatedConfirmation = true;
                    }
                }
        }

        public void OnPostActivateCreateEmployee()
        {
            IsEditMode = true;
            CanSetPassword = true;
            IsChoosen = true;
            IsChoosenToCreate = true;
        }
        public void OnPostCreateEmployee()
        {
            Employee newemployee = new Employee
            {
                FirstName = Choosen_FirstName,
                LastName = Choosen_LastName,
                Mail = Choosen_Mail,
                PhoneNumber = Choosen_PhoneNumber,
                Password = Choosen_Password,
                Gender = Choosen_Gender,
                ImagePath = Choosen_ImagePath,
                Address = new Address(0, 0, Choosen_StreetName, Choosen_HouseNumber, new City(0, 0, Choosen_City, new ZipCodeTable(0, Choosen_ZipCode))),
                Position = new Position(0,Choosen_Title,Choosen_PositionDescription,Choosen_AccessLevel),
                Department = new Department(0,Choosen_DepartmentName,Choosen_DepartmentDescription),
            };
            ShowEmployee = _employeeRepository.Create(newemployee);

            EmployeeLog Log = new EmployeeLog
            {
                ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                Date = DateTime.Now,
                Activity = _employeeRepository.LogText,
                Type = "Employee Created",
                AffectedEmployeeId = newemployee.EmployeeId,
            };
            _logRepository.Create(Log);

            Employees = _employeeRepository.GetAll();
            IsCreatedConfirmation = true;
        }
    }

}
