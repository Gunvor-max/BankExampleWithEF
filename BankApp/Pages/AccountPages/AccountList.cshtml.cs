using BankLib.Model;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace BankApp.Pages.AccountPages
{
    public class AccountListModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        public AccountListModel(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            Accounts = _accountRepository.GetAll();
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
        public string Choosen_AccountName {  get; set; }
        [BindProperty]
        public string Choosen_AccountType { get; set; }

        public bool IsEditMode { get; set; } = false;
        public bool IsDeleteable { get; set; } = false;
        public bool IsDeletedConfirmation { get; set; } = false;
        public Account ShowAccount { get; set; }
        public bool IsCustomerAgent { get; set; }
        public bool IsEditTriggered { get; set; } = false;
        public bool IsCreatedConfirmation { get; set; } = false;
        public bool IsUpdatedConfirmation { get; set; } = false;
        public bool CanSetPassword { get; set; } = false;
        public bool IsChoosen { get; set; } = false;
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
                Accounts = _accountRepository.GetAll();
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
            if(Search is not null)
            {
            var listcustomers = _customerRepository.Search(Search);
            if (listcustomers.Count != 0)
            {
                foreach (var customer in listcustomers)
                {
                    Accounts = _accountRepository.ReadAccountsConnectedToMain(customer.MainAccountId);
                    return Page();
                }
            } 
            }
                Accounts = _accountRepository.Search(Search);
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
            IsChoosen = true;
        }

        public void OnPostEnableEdit(int accountId)
        {

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
            IsChoosen = true;
        }

        public void OnPostDeleteAccount(int id)
        {
            Account account = _accountRepository.Read(id);
            account.IsDeleted = true;
            ShowAccount = _accountRepository.Update(account, id);
            Accounts = _accountRepository.GetAll();
            IsDeletedConfirmation = true;
        }

        public void OnPostUpdateAccount(int accountId)
        {
            Account choosenAccount = _accountRepository.Read(accountId);

            if (accountId != 0)
            {
                choosenAccount.Name = Choosen_AccountName;
                choosenAccount.Type = Choosen_AccountType;
                ShowAccount = _accountRepository.Update(choosenAccount, accountId);
                Accounts = _accountRepository.GetAll();
                IsUpdatedConfirmation = true;
            }
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
