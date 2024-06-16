using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BankLib.Model;
using System.Net;

namespace BankApp.Pages.EmployeePages
{
    public class EmployeeListModel : PageModel
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeListModel(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            Employees = _employeeRepository.GetAll();
        }
        public List<Employee> Employees { get; set; }

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
        public void OnGet()
        {
            //Check if the user has the proper accesslevel to view the page
            object user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                Employee employee = null;
                employee = SessionHelper.Get<Employee>(employee, HttpContext);
                if (employee.Position.AccessLevel < 4)
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

        public IActionResult OnPostSearchEmployees()
        {
            return Page();
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
        }

        public void OnPostEnableEdit(int Id)
        {
            bool isEmployee = true;

                Employee employee = _employeeRepository.Read(Id);

                if (Id != 0)
                {
                    if (employee is not null)
                    {
                        EmployeeId = employee.EmployeeId;
                    }
                }

            IsEditMode = true;
            IsEditTriggered = true;
        }

        public void OnPostDeleteEmployee(int id)
        {
            IsDeletedConfirmation = true;
        }

        public void OnPostUpdateEmployee(int id)
        {
            bool isEmployee = true;

                Employee employee = _employeeRepository.Read(id);

                if (id != 0)
                {
                    if (employee is not null)
                    {
                        employee.FirstName = Choosen_FirstName;
                        employee.LastName = Choosen_LastName;
                        employee.Mail = Choosen_Mail;
                        employee.PhoneNumber = Choosen_PhoneNumber;
                        employee.Address.StreetName = Choosen_StreetName;
                        employee.Address.HouseNumber = Choosen_HouseNumber;
                        employee.Address.City.CityName = Choosen_City;
                        employee.Address.City.ZipCode.ZipCode = Choosen_ZipCode;
                        ShowEmployee = _employeeRepository.Update(employee, id);
                        Employees = _employeeRepository.GetAll();
                        IsUpdatedConfirmation = true;
                    }
                }
        }

        public void OnPostActivateCreateCustomerAgent()
        {
            IsEditMode = true;
            IsCustomerAgent = true;
            CanSetPassword = true;
        }
        public void OnPostActivateCreateEmployee()
        {
            IsEditMode = true;
            CanSetPassword = true;
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
            IsCreatedConfirmation = true;
        }
    }

}
