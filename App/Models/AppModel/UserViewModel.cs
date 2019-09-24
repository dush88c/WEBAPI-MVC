using System;

namespace App.Models.AppModel
{
    public class UserViewModel
    {
        public string FullName => $"{FirstName}{" "}{LastName}";

        public string Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
                
        public string ProfileUrl { get; set; }
       
        public char Gender { get; set; }
        
        public string Email { get; set; }
        
        public string AddressLine01 { get; set; }
        
        public string AddressLine02 { get; set; }
        
        public string City { get; set; }
       
        public string HomePhoneNumber { get; set; }
        
        public string MobileNumber { get; set; }
        
        public string UserRoleId { get; set; }
        
        public DateTime? BirthOfDate { get; set; }

        public string BirthOfDateDisplayName { get; set; }
        
        public string Password { get; set; }
       
        public string ConfirmPassword { get; set; }
        
        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string Token { get; set; }
    }
}