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
        private readonly ITransactionRepository _transactionRepository;

        public OverviewModel(ICustomerRepository customerRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            //Get acounts connected to either employee or customer
            if (employeeLoggedIn is not null || customerLoggedIn is not null)
            {
                var customerOrEmployee = employeeLoggedIn is not null ? _accountRepository.ReadAccountsConnectedToMain(employeeLoggedIn.MainAccountId) :
                                                            _accountRepository.ReadAccountsConnectedToMain(customerLoggedIn.MainAccountId);
                Accounts = customerOrEmployee;
            }
            _transactionRepository = transactionRepository;
        }
        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public static Employee? employeeLoggedIn { get; set; }
        public static Customer? customerLoggedIn { get; set; }

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
        public bool TransforSuccess { get; set; } = false;
        public bool SeeTransactions {  get; set; } = false;
        public bool SeeGraph { get; set; } = false;
        public List<DateTime> TransactionDates { get; set; }
        public List<int> TransactionBalance {  get; set; }
        public List<string> FormattedDates { get; set; }
        public void OnGet()
        {
            //Check if the user has the proper accesslevel to view the page
            object user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                employeeLoggedIn = SessionHelper.Get<Employee>(employeeLoggedIn, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(employeeLoggedIn.MainAccountId);
            }
            else if(SessionHelper.Get<object>(user, HttpContext) is not null)
            {
                customerLoggedIn = SessionHelper.Get<Customer>(customerLoggedIn, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(customerLoggedIn.MainAccountId);
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

        public void OnPostSeeTransactionsForAccount(int accountId) 
        {
            Transactions = _transactionRepository.ReadAccountTransactions(accountId);
            SeeTransactions = true;
        }

        public void OnPostTransactionsGraph(int accountId)
        {
            Transactions = _transactionRepository.ReadAccountTransactions(accountId);
            TransactionDates = [];
            TransactionBalance = [];
            foreach (Transaction transaction in Transactions) 
            {
                TransactionDates.Add(transaction.Date);
                TransactionBalance.Add(transaction.Current_Balance);
                
            }
            FormattedDates = TransactionDates.Select(date => date.ToString("dd/MM - HH:mm")).ToList();
            SeeTransactions = false;
            SeeGraph = true;
        }
    }
}
