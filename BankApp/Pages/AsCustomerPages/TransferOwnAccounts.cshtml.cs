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
            Account_ChosenFrom = _accountRepository.ReadAccountsConnectedToMain(MainidFrom).FirstOrDefault(a => a.AccountId == accountidFrom);
            // Store chosen account in session
            HttpContext.Session.SetString("ChosenAccount", JsonConvert.SerializeObject(Account_ChosenFrom));
            IsChosenFrom = true; // Update flag after assigning the chosen account
            return Page();
        }
        public IActionResult OnPostChoosenAccountTo(int accountidTo, int MainidTo)
        {
            Account_ChosenTo = _accountRepository.ReadAccountsConnectedToMain(MainidTo).FirstOrDefault(a => a.AccountId == accountidTo);
            // Retrieve chosen account from session
            string chosenAccountJson = HttpContext.Session.GetString("ChosenAccount");

            if (chosenAccountJson != null)
            {
                Account_ChosenFrom = JsonConvert.DeserializeObject<Account>(chosenAccountJson);
            }
            HttpContext.Session.SetString("ChosenAccount1", JsonConvert.SerializeObject(Account_ChosenTo));

            IsChosenFrom = true; // Update flag after assigning the chosen account
            IsChosenTo = true;
            return Page();
        }

        public IActionResult OnPostTransfer()
        {
            string chosenAccountJson = HttpContext.Session.GetString("ChosenAccount");

            if (chosenAccountJson != null)
            {
                var chosenAccountFrom = JsonConvert.DeserializeObject<Account>(chosenAccountJson);

                    // Update existing account
                    _accountRepository.Withdraw(chosenAccountFrom.AccountId, DepositAmount); // Assuming WithDrawMoney updates the Amount
            }

            string chosenAccountJson1 = HttpContext.Session.GetString("ChosenAccount1");

            if (chosenAccountJson1 != null)
            {
                var chosenAccountTo = JsonConvert.DeserializeObject<Account>(chosenAccountJson1);

                // Update existing account
                _accountRepository.Deposit(chosenAccountTo.AccountId, DepositAmount); // Assuming WithDrawMoney updates the Amount
            }
            Transforsuccess = true;
            TempData["transforsuccess"] = Transforsuccess;
            return RedirectToPage("/AsCustomerPages/Overview");
        }
    }
}
