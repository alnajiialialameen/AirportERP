using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AirPortERP.Models
{
    public class RegisterViewModel
    {

        public string Id { get; set; }
        [Required]
      
        public string UserType { get; set; }


        public int CompanyInfo { get; set; }


        [Required]
        public string UserName { get; set; }


        public string OldUserName { get; set; }

        //[Required]
        //[EmailAddress(ErrorMessage = "Email Address Is Invaild")]
        public string Email { get; set; }

        [Required]

        public string Password { get; set; }

        public string OldPassword { get; set; }
        public string OldEmail { get; set; }


        [Compare("Password", ErrorMessage = "Confirm Does'nt Match")]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }
    }

    public class LoginViewModel
    {
       

        [Required]
        public string UserName { get; set; }

        [Required]

        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string Message { get; set; }
    }



    public class AirportViewModel
    {

        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public int CompanyInfo { get; set; }   
        public string companyInfoName { get; set; }
        public int CompanyInfoId { get; set; }

      
    }









}
