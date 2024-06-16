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
            Employee employee = null!;
            SessionHelper.Get(employee, HttpContext);
            SessionHelper.Clear(employee, HttpContext);

            return Redirect("/Index");
        }
    }
}
