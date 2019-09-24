using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace SurvayArm.API.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName));
            userIdentity.AddClaim(new Claim("LastName", this.LastName));
            userIdentity.AddClaim(new Claim("FullName", this.FullName));
            userIdentity.AddClaim(new Claim("MobileNumber", this.PhoneNumber));
            userIdentity.AddClaim(new Claim("Address", this.Address));
            userIdentity.AddClaim(new Claim("Id", this.Id));

            return userIdentity;
            
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public char Gender { get; set; }

        public string ProfileImgUrl { get; set; }

        public DateTime? BirthOfDate { get; set; }

        public string AddressLine01 { get; set; }

        public string AddressLine02 { get; set; }

        public string City { get; set; }

        public string HomePhoneNumber { get; set; }

        public string Password { get; set; }

        public System.DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string FullName => $"{FirstName}{" "}{LastName}";

        public string Address => $"{AddressLine01}{" "}{AddressLine02}{" "}{City}";
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}