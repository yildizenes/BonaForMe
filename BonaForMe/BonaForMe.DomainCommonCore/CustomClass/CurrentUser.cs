using System;
using System.Security.Claims;

namespace BonaForMe.DomainCommonCore.CustomClass
{
    public class CurrentUser
    {
        public static UserClaimsInfo Get(ClaimsPrincipal User)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return new UserClaimsInfo()
                    {
                        UserId = User?.FindFirst(ClaimTypes.PrimarySid).Value,
                        UserName = User?.FindFirst(ClaimTypes.Name).Value,
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }      
        }
    }
}