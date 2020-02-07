using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
