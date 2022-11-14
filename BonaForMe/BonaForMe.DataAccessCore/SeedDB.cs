using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BonaForMe.DataAccessCore
{
    public class SeedDB
    {
        public SeedDB()
        {

        }
        public static void Initialize(BonaForMeDBContext context)
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();

            context.SaveChanges();
        }
    }
}