using Lamar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BonaForMe.DataAccessCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonaForMe.API.Infrastucture
{
    public class LamarMainRegistry:ServiceRegistry
    {
        public LamarMainRegistry(IConfiguration configuration)
        {
            Scan(x =>
            {
                x.Assembly(typeof(Program).Assembly);
                x.WithDefaultConventions();
                x.Assembly("BonaForMe.Api");
                x.Assembly("BonaForMe.ServiceCore");
                x.Assembly("BonaForMe.DomainCore");

            });

            var connectionString = configuration.GetConnectionString("InovationBusinessDBContext");
            var optionsBuilder = new DbContextOptionsBuilder<BonaForMeDBContext>();  // Contextin İsmi Gelecek
            optionsBuilder.UseSqlServer(connectionString);


            For<BonaForMeDBContext>().Use<BonaForMeDBContext>()
                  .Ctor<DbContextOptions<BonaForMeDBContext>>("options")
                              .Is(optionsBuilder.Options);
        }
    }
}
