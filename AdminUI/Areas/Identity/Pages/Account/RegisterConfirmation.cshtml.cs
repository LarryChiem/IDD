using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using AdminUI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AdminUI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<AdminUIUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<AdminUIUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender sender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            //TODO: Uncomment out this block of code and replace 'SendEmailTo' with whatever function Multnomah County
            //uses to send emails, then remove the block of code below it
            /*
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var subject = user.Name + " is requesting an account on AdminUI";
            var body = "<p><b>" + user.Name + "</b> is requesting an account on AdminUI with the Role <b>" +
                       await _userManager.GetRolesAsync(user) + "</b></p>." +  
                       "<p>Please click the following link to confirm: <a href=" + code + ">Confirm Account</a></p>";
            foreach (var admin in await _userManager.GetUsersInRoleAsync("Administrator"))
                SendEmailTo(admin.Email, subject, body);
            */

            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);
            }

            return Page();
        }
    }
}
