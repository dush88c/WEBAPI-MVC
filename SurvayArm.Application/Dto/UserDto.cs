using System;

namespace SurvayArm.Application.Dto
{
   public class UserDto 
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthOfDate { get; set; }
        public string AddressLine01 { get; set; }
        public string AddressLine02 { get; set; }
        public string City { get; set; }
        public string HomePhoneNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
    }
}
