using BonaForMe.DomainCore.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BonaForMe.ServicesCore.JwtService
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
