using BankLib.Model;
using BankLib.Services;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BankApp.Pages.AsCustomerPages
{
    public class TransferOwnAccountsModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        public TransferOwnAccountsModel(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            //Get acounts connected to either employee or customer
            if (EmployeeLoggedIn is not null || CustomerLoggedIn is not null)
            {
                var customerOrEmployee = EmployeeLoggedIn is not null ? _accountRepository.ReadAccountsConnectedToMain(EmployeeLoggedIn.MainAccountId) :
                                                                        _accountRepository.ReadAccountsConnectedToMain(CustomerLoggedIn.MainAccountId);
                Accounts = customerOrEmployee;
            }
        }

        public List<Account> Accounts { get; set; }
        public Account? Account_ChosenFrom { get; set; }
        public Account? Account_ChosenTo { get; set; }
        public static Account? AccountFromStored { get; set; }
        public static Account? AccountToStored { get; set; }
        public bool IsChosenFrom { get; set; } = false;
        public bool IsChosenTo { get; set; } = false;
        public bool Transforsuccess { get; set; } = false;
        public static Customer? CustomerLoggedIn { get; set; }
        public static Employee? EmployeeLoggedIn { get; set; }
        [BindProperty]
        public int DepositAmount {  get; set; }

        public void OnGet()
        {
            //Check if the user has the proper accesslevel to view the page
            object user = null;
            if (SessionHelper.Get<object>(user, HttpContext)?.ToString()?.Contains("AccessLevel") ?? false)
            {
                //Session is retrieved as an employee object, and further restrictions can be set depending on accesslevel
                EmployeeLoggedIn = SessionHelper.Get<Employee>(EmployeeLoggedIn, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(EmployeeLoggedIn.MainAccountId);
            }
            else if (SessionHelper.Get<object>(user, HttpContext) is not null)
            {
                CustomerLoggedIn = SessionHelper.Get<Customer>(CustomerLoggedIn, HttpContext);
                Accounts = _accountRepository.ReadAccountsConnectedToMain(CustomerLoggedIn.MainAccountId);
            }
            else
            {
                Response.Redirect("/LoginPages/Login");
            }
        }

        public IActionResult OnPostChoosenAccountFrom(int accountidFrom, int MainidFrom)
        {
            //retrieve AccountFrom and store it in a static object
            AccountFromStored = _accountRepository.ReadAccountsConnectedToMain(MainidFrom).FirstOrDefault(a => a.AccountId == accountidFrom);

            //Assign stored Account in the static object and assign it to be displayed in the view
            Account_ChosenFrom = AccountFromStored;

            IsChosenFrom = true; 
            return Page();
        }
        public IActionResult OnPostChoosenAccountTo(int accountidTo, int MainidTo)
        {
            //retrieve AccountFrom to display in the View
            Account_ChosenFrom = AccountFromStored;

            //retrieve AccountTo and store it in a static object
            AccountToStored = _accountRepository.ReadAccountsConnectedToMain(MainidTo).FirstOrDefault(a => a.AccountId == accountidTo);
            //Assign stored Account in the static object and assign it to be displayed in the view
            Account_ChosenTo = AccountToStored;

            IsChosenFrom = true;
            IsChosenTo = true;
            return Page();
        }

        public IActionResult OnPostTransfer()
        {
            //Account to withdraw money from
            _accountRepository.Withdraw(AccountFromStored.AccountId, DepositAmount); 
            
            //Account to deposit money
            _accountRepository.Deposit(AccountToStored.AccountId, DepositAmount); 

            Transforsuccess = true;
            TempData["transforsuccess"] = Transforsuccess;
            return RedirectToPage("/AsCustomerPages/Overview");
        }
    }
}
