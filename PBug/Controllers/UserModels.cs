using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PBug.Controllers
{
    public class LoginRequest
    {
        [BindProperty(Name = "username"), BindRequired]
        public string Username { get; set; }

        [BindProperty(Name = "password"), BindRequired]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [BindProperty(Name = "inviteid"), BindRequired]
        public string InviteID { get; set; }

        [BindProperty(Name = "fullname"), BindRequired]
        [StringLength(maximumLength: 100, MinimumLength = 6)]
        public string FullName { get; set; }

        [BindProperty(Name = "username"), BindRequired]
        [StringLength(maximumLength: 64, MinimumLength = 3)]
        public string Username { get; set; }

        [BindProperty(Name = "password"), BindRequired]
        [StringLength(maximumLength: int.MaxValue, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        [BindProperty(Name = "oldpassword"), BindRequired]
        public string OldPassword { get; set; }

        [BindProperty(Name = "newpassword"), BindRequired]
        [StringLength(maximumLength: int.MaxValue, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}