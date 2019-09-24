namespace SurvayArm.Utility
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this System.Security.Principal.IIdentity identity)
        {
            var claim = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("FirstName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLastName(this System.Security.Principal.IIdentity identity)
        {
            var claim = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("LastName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserFullName(this System.Security.Principal.IIdentity identity)
        {
            var claimFirstName = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("FirstName");
            var claimLastName = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("LastName");
            // Test for null to avoid issues during local testing
            return (claimFirstName != null && claimLastName != null) ? $"{claimFirstName.Value}  {claimLastName.Value} " : string.Empty;
        }

        public static string GetUserMobileNumber(this System.Security.Principal.IIdentity identity)
        {
            var claimMobileNo = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("MobileNumber");

            return (claimMobileNo != null) ? claimMobileNo.Value : string.Empty;
        }

        public static string GetUserId(this System.Security.Principal.IIdentity identity)
        {
            var claimId = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("Id");

            return claimId.Value.ToString();
        }

        public static string GetUserAddress(this System.Security.Principal.IIdentity identity)
        {
            var claimAddress = ((System.Security.Claims.ClaimsIdentity)identity).FindFirst("Address");

            return (claimAddress != null) ? claimAddress.Value : string.Empty;
        }

    }
}

