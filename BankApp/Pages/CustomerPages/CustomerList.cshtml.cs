using BankLib.Model;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.CustomerPages
{
    public class CustomerListModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        public CustomerListModel(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            Customers = _customerRepository.GetAll();
        }
        public List<Customer> Customers { get; set; }

        [BindProperty]
        public string? Search { get; set; }

        //Customerbind
        [BindProperty]
        public int CustomerID { get; set; }
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

        //Addressbind
        [BindProperty]
        public string Choosen_StreetName { get; set; }
        [BindProperty]
        public int Choosen_HouseNumber { get; set; }
        [BindProperty]
        public string Choosen_City { get; set; }
        [BindProperty]
        public int Choosen_ZipCode { get; set; }

        //AccountBind
        [BindProperty]
        public string Choosen_AccountName { get; set; }
        [BindProperty]
        public string Choosen_AccountType { get; set; }

        public bool IsEditMode { get; set; } = false;
        public bool IsDeleteable { get; set; } = false;
        public bool IsDeletedConfirmation { get; set; } = false;
        public Customer ShowCustomer { get; set; }
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
            object? user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                Employee? employee = null;
                employee = SessionHelper.Get<Employee>(employee, HttpContext);
                if (employee?.Position.AccessLevel < 4)
                {
                    Response.Redirect("/LoginPages/Login");
                }
                Customers = _customerRepository.GetAll();
            }
            else
            {
                Response.Redirect("/LoginPages/Login");
            }
        }

        public void OnPostSearchCustomers()
        {
            Customers = _customerRepository.Search(Search);
        }

        public void OnPostSelectCustomer(int Id)
        {
            Customer customer = _customerRepository.Read(Id);

            if (Id != 0)
            {
                if (customer is not null)
                {
                    //Customer
                    CustomerID = customer.CustomerId;
                    Choosen_FirstName = customer.FirstName;
                    Choosen_LastName = customer.LastName;
                    Choosen_Mail = customer.Mail;
                    Choosen_PhoneNumber = customer.PhoneNumber;
                    Choosen_Gender = customer.Gender;
                    Choosen_Password = customer.Password;
                    //Address
                    Choosen_StreetName = customer.Address.StreetName;
                    Choosen_HouseNumber = customer.Address.HouseNumber;
                    Choosen_City = customer.Address.City.CityName;
                    Choosen_ZipCode = customer.Address.City.ZipCode.ZipCode;
                }
            }
            IsEditMode = false;
            IsDeleteable = true;
            IsChoosen = true;
        }

        public void OnPostEnableEdit(int Id)
        {
            bool isEmployee = true;

            Customer customer = _customerRepository.Read(Id);

            if (Id != 0)
            {
                if (customer is not null)
                {
                    CustomerID = customer.CustomerId;
                }
            }

            IsEditMode = true;
            IsEditTriggered = true;
            IsChoosen = true;
        }

        public void OnPostDeleteCustomer(int id)
        {
            IsDeletedConfirmation = true;
        }

        public void OnPostUpdateCustomer(int id)
        {
            bool isEmployee = true;

            Customer customer = _customerRepository.Read(id);

            if (id != 0)
            {
                if (customer is not null)
                {
                    customer.FirstName = Choosen_FirstName;
                    customer.LastName = Choosen_LastName;
                    customer.Mail = Choosen_Mail;
                    customer.PhoneNumber = Choosen_PhoneNumber;
                    customer.Address.StreetName = Choosen_StreetName;
                    customer.Address.HouseNumber = Choosen_HouseNumber;
                    customer.Address.City.CityName = Choosen_City;
                    customer.Address.City.ZipCode.ZipCode = Choosen_ZipCode;
                    ShowCustomer = _customerRepository.Update(customer, id);
                    Customers = _customerRepository.GetAll();
                    IsUpdatedConfirmation = true;
                }
            }
        }

        public void OnPostActivateCreateCustomerAgent()
        {
            IsEditMode = true;
            IsCustomerAgent = true;
            CanSetPassword = true;
            IsChoosen = true;
            IsChoosenToCreate = true;
        }
        public void OnPostActivateCreateCustomer()
        {
            IsEditMode = true;
            CanSetPassword = true;
            IsChoosen = true;
            IsChoosenToCreate = true;
        }
        public void OnPostCreateCustomer()
        {
            Customer newcustomer = new Customer
            {
                FirstName = Choosen_FirstName,
                LastName = Choosen_LastName,
                Mail = Choosen_Mail,
                PhoneNumber = Choosen_PhoneNumber,
                Password = Choosen_Password,
                Gender = Choosen_Gender,
                Address = new Address(0, 0, Choosen_StreetName, Choosen_HouseNumber, new City(0, 0, Choosen_City, new ZipCodeTable(0, Choosen_ZipCode))),
            };
            ShowCustomer = _customerRepository.Create(newcustomer);
            Customers = _customerRepository.GetAll();
            IsCreatedConfirmation = true;
        }

        public void OnPostCreateAccount(int id) 
        {
            var customer = _customerRepository.Read(id);
            Account newaccount = new Account
            {
                Name = Choosen_AccountName,
                Type = Choosen_AccountType,
                MainAccountId = customer.MainAccountId
            };

            _accountRepository.Create(newaccount);
        }
    }
}
