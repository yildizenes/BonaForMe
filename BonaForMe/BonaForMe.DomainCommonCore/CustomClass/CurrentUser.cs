using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace BonaForMe.DomainCommonCore.CustomClass
{
    public class CurrentUser
    {
        public static UserClaimsInfo Get(ClaimsPrincipal User)
        {
            try
            {
                if (User.Identity.IsAuthenticated != false)
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