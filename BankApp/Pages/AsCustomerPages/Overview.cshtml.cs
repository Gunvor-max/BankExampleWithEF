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

        public void OnPostSearchAccounts()
        {
            Accounts = _accountRepository.Search(Search, customerLoggedIn.MainAccountId);
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
            FormattedDates = TransactionDates.Select(date => date.ToString("d\\/M - 'kl.'HH:mm")).ToList();
            SeeTransactions = false;
            SeeGraph = true;
        }
    }
}
