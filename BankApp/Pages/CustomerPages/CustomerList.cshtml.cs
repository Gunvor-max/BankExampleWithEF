using BankLib.Model;
using BankLib.Services;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Pages.CustomerPages
{
    public class CustomerListModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogRepository _logRepository;
        public CustomerListModel(ICustomerRepository customerRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, ILogRepository logRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _logRepository = logRepository;
            Customers = _customerRepository.GetAll();
        }
        public List<Customer> Customers { get; set; }
        public static Employee? EmployeeLoggedIn { get; set; }
        public string AccountLogText { get; set; }
        public List<Account> Accounts { get; set; }

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
                EmployeeLoggedIn = SessionHelper.Get<Employee>(EmployeeLoggedIn, HttpContext);
                if (EmployeeLoggedIn?.Position.AccessLevel < 4)
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
            Accounts = _accountRepository.ReadAccountsConnectedToMain(customer.MainAccountId);

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
            Customer customer = _customerRepository.Read(id);
            List<Account> accounts = _accountRepository.ReadAccountsConnectedToMain(customer.MainAccountId);

            //Delete customer
            customer.IsDeleted = true;
            ShowCustomer = _customerRepository.Delete(customer);


            //Delete Accounts connected to customer
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
                    Activity = _customerRepository.LogText + AccountLogText,
                    Type = "Customer and Accounts Deleted",
                    AffectedCustomerId = customer.CustomerId,
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
                    Activity = _customerRepository.LogText,
                    Type = "Customer Deleted",
                    AffectedCustomerId = customer.CustomerId,
                };

                _logRepository.Create(Log);
            }
            Customers = _customerRepository.GetAll();
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

                    //Create log
                    EmployeeLog Log = new EmployeeLog
                    {
                        ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                        Date = DateTime.Now,
                        Activity = _customerRepository.LogText,
                        Type = "Customer Updated",
                        AffectedCustomerId = customer.CustomerId,
                    };
                    _logRepository.Create(Log);
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

            EmployeeLog Log = new EmployeeLog
            {
                ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                Date = DateTime.Now,
                Activity = _customerRepository.LogText,
                Type = "Customer Created",
                AffectedCustomerId = newcustomer.CustomerId,
            };
            _logRepository.Create(Log);
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

            Transaction starttransaction = new Transaction
            {
                CustomerId = customer.CustomerId,
                AccountId = newaccount.AccountId,
                Amount = 0,
                Date = DateTime.Now,
                Type = "Oprettet",
                Description = "Konto oprettet",
                Current_Balance = newaccount.Balance
            };
            _transactionRepository.Create(starttransaction);

            EmployeeLog Log = new EmployeeLog
            {
                ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                Date = DateTime.Now,
                Activity = _accountRepository.LogText,
                Type = "Account Created",
                AffectedCustomerId = customer.CustomerId,
            };
            _logRepository.Create(Log);
            IsCreatedConfirmation = true;
        }
    }
}
