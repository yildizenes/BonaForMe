using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BonaForMe.DomainCommonCore.Enums
{
    public class AppEnums
    {
        public enum LogTypes
        {
            [Description("Success")]
            Success = 1,
            [Description("UnSuccess")]
            UnSuccess = 2,
            [Description("Error")]
            Error = 3,
            [Description("History")]
            History = 4,
            [Description("All")]
            All = 5
        }

        public enum UserTypes
        {
            [Description("Success")]
            SuperAdmin = 1,
            [Description("UnSuccess")]
            User = 2,
        }
    }
}
