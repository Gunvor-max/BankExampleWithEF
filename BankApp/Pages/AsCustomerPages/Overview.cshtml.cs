using BankLib.Model;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.AsCustomerPages
{
    public class OverviewModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;

        public OverviewModel(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }
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


        //Accountbind
        [BindProperty]
        public int AccountID { get; set; }
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
        public void OnGet()
        {
            //Check if the user has the proper accesslevel to view the page
            object user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                Employee? employee = null;
                employee = SessionHelper.Get<Employee>(employee, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(employee.MainAccountId);
            }
            else if(SessionHelper.Get<object>(user, HttpContext) is not null)
            {
                Customer? customer = null;
                customer = SessionHelper.Get<Customer>(customer, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(customer.MainAccountId);
            }
            else
            {
                Response.Redirect("/LoginPages/Login");
            }
        }

        public Customer GetCustomerFromMainAccount(int mainAccountId)
        {
            var customer = _customerRepository.GetAll().FirstOrDefault(ma => ma.MainAccountId == mainAccountId);
            return customer;
        }

        public IActionResult OnPostSearchAccounts()
        {
            var listcustomers = _customerRepository.Search(Search);
            foreach (var customer in listcustomers)
            {
                Accounts = _accountRepository.ReadAccountsConnectedToMain(customer.MainAccountId);
                return Page();
            }
            Accounts = new List<Account>();
            return Page();
        }

        public void OnPostSelectAccount(int accountId)
        {
            Account choosenAccount = _accountRepository.Read(accountId);

            if (accountId != 0)
            {
                if (choosenAccount is not null)
                {
                    //Customer
                    var customer = _customerRepository.GetAll().First(ma => ma.MainAccountId == choosenAccount.MainAccountId);
                    CustomerID = customer.CustomerId;
                    Choosen_FirstName = customer.FirstName;
                    Choosen_LastName = customer.LastName;

                    //Account
                    AccountID = choosenAccount.AccountId;
                    Choosen_AccountName = choosenAccount.Name;
                    Choosen_AccountType = choosenAccount.Type;
                }
            }
            IsEditMode = false;
            IsDeleteable = true;
        }

        public void OnPostEnableEdit(int accountId)
        {
            bool isEmployee = true;

            Account choosenAccount = _accountRepository.Read(accountId);
            //Customer
            var customer = _customerRepository.GetAll().First(ma => ma.MainAccountId == choosenAccount.MainAccountId);
            CustomerID = customer.CustomerId;
            Choosen_FirstName = customer.FirstName;
            Choosen_LastName = customer.LastName;

            //Account
            AccountID = choosenAccount.AccountId;
            Choosen_AccountName = choosenAccount.Name;
            Choosen_AccountType = choosenAccount.Type;

            IsEditMode = true;
            IsEditTriggered = true;
        }

        public void OnPostDeleteCustomer(int id)
        {
            IsDeletedConfirmation = true;
        }

        public void OnPostUpdateCustomer(int id)
        {
            //bool isEmployee = true;

            //Customer customer = _customerRepository.Read(id);

            //if (id != 0)
            //{
            //    if (customer is not null)
            //    {
            //        customer.FirstName = Choosen_FirstName;
            //        customer.LastName = Choosen_LastName;
            //        customer.Mail = Choosen_Mail;
            //        customer.PhoneNumber = Choosen_PhoneNumber;
            //        customer.Address.StreetName = Choosen_StreetName;
            //        customer.Address.HouseNumber = Choosen_HouseNumber;
            //        customer.Address.City.CityName = Choosen_City;
            //        customer.Address.City.ZipCode.ZipCode = Choosen_ZipCode;
            //        ShowCustomer = _customerRepository.Update(customer, id);
            //        Customers = _customerRepository.GetAll();
            //        IsUpdatedConfirmation = true;
            //    }
            //}
        }

        public void OnPostActivateCreateCustomerAgent()
        {
            IsEditMode = true;
            IsCustomerAgent = true;
            CanSetPassword = true;
        }
        public void OnPostActivateCreateCustomer()
        {
            IsEditMode = true;
            CanSetPassword = true;
        }
        public void OnPostCreateCustomer()
        {
            //Customer newcustomer = new Customer
            //{
            //    FirstName = Choosen_FirstName,
            //    LastName = Choosen_LastName,
            //    Mail = Choosen_Mail,
            //    PhoneNumber = Choosen_PhoneNumber,
            //    Password = Choosen_Password,
            //    Gender = Choosen_Gender,
            //    Address = new Address(0, 0, Choosen_StreetName, Choosen_HouseNumber, new City(0, 0, Choosen_City, new ZipCodeTable(0, Choosen_ZipCode))),
            //};
            //ShowCustomer = _customerRepository.Create(newcustomer);
            //Customers = _customerRepository.GetAll();
            //IsCreatedConfirmation = true;
        }
    }
}
