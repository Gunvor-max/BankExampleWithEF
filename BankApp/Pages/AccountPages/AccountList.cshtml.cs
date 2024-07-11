using BankLib.Model;
using BankLib.Services;
using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Pages.AccountPages
{
    public class AccountListModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogRepository _logRepository;
        public AccountListModel(ICustomerRepository customerRepository, IAccountRepository accountRepository, ILogRepository logRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _logRepository = logRepository;
            Accounts = _accountRepository.GetAll();
        }
        public List<Account> Accounts { get; set; }
        public static Employee? EmployeeLoggedIn { get; set; }

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

        [Required(ErrorMessage = "Konto navn er påkrævet")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Konto navn skal være mellem 2-30 tegn")]
        [BindProperty]
        public string Choosen_AccountName {  get; set; }

        [Required(ErrorMessage = "Konto type er påkrævet")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Konto type skal være mellem 2-30 tegn")]
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
                EmployeeLoggedIn = SessionHelper.Get<Employee>(EmployeeLoggedIn, HttpContext);
                if (EmployeeLoggedIn.Position.AccessLevel < 4)
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

            ModelState.Remove("Choosen_AccountType");
            ModelState.Remove("Choosen_AccountName");

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

            if (ModelState.IsValid && accountId != 0)
            {
                choosenAccount.Name = Choosen_AccountName;
                choosenAccount.Type = Choosen_AccountType;
                ShowAccount = _accountRepository.Update(choosenAccount, accountId);

                //Create log
                EmployeeLog Log = new EmployeeLog
                {
                    ResponsibleEmployeeId = EmployeeLoggedIn.EmployeeId,
                    Date = DateTime.Now,
                    Activity = _accountRepository.LogText,
                    Type = "Account Updated",
                    AffectedCustomerId = GetCustomerFromMainAccount(choosenAccount.MainAccountId).CustomerId,
                };
                _logRepository.Create(Log);

                Accounts = _accountRepository.GetAll();
                IsUpdatedConfirmation = true;
            }
            else 
            {
                OnPostEnableEdit(accountId);
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
    }
}
