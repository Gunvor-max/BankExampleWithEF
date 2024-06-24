using BankLib.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApp.Pages.LoginPages
{
    public class LogoutModel : PageModel
    {
        public LogoutModel()
        {
        }

        public IActionResult OnGet()
        {
            object? user = null;
            user = SessionHelper.Get<object>(user, HttpContext);
            if (user.ToString().Contains("AccessLevel"))
            {
                Employee employee = null!;
                SessionHelper.Clear(employee, HttpContext);
            }
            else
            {
                Customer customer = null!;
                SessionHelper.Clear(customer, HttpContext);
            }

            return Redirect("/Index");
        }
    }
}
