using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zamm.Application.Payloads.RequestModels.UserRequests
{
    public class RegisterInput
    {
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")] 
        public string Password { get; set; }
        [Required(ErrorMessage = "Full name is required")] 
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
