using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"))!.Value;
        }//From Token service and claims -> predefined identifier for the claim type “GivenName” in the Microsoft identity system.
    }
}