using BankLib.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Pages.LoginPages
{
    public class LoginModel : PageModel
    {
        private readonly IEmployeeRepository _employeeRepository;

        [Required(ErrorMessage = "Email er påkrævet.")]
        [EmailAddress(ErrorMessage = "Indtast en gyldig email.")]
        [BindProperty]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password er påkrævet.")]
        [BindProperty]
        public string Password { get; set; }
        public bool IsCustomer { get; set; } = true;

        public string ErrorMessage { get; set; }

        public LoginModel(IEmployeeRepository employeeRepository)
        {
          _employeeRepository = employeeRepository;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostLogin()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else if (_employeeRepository.CheckUser(Email, Password))
            {
                //If true sets session from the user object in IsloggedIn
                SessionHelper.Set(_employeeRepository.Isloggedin, HttpContext);
                return RedirectToPage("/Index");
            }
            
            ErrorMessage = "Ugyldig login. Prøv igen.";

            return Page();
        }
    }
}
