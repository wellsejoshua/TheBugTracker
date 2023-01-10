using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace TheBugTracker.Extensions
{
    public static class IdentityExtensions
    {
        public static int? GetCompanyId(this IIdentity identity)
        {
            //Claims Identity implements the IIdentity
            //identity is cast to claims identity to allow utilization of FindFirst() that allow
            //you to look into the list of claims and find CompanyId
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId");
            //ternary operator(if/else)
            return (claim != null) ? int.Parse(claim.Value) : null;


        }
    }
}
