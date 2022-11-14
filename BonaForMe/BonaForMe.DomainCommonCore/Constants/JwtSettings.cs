﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BonaForMe.DomainCommonCore.Constants
{
    public class JwtSettings
    {
        public static string SecretKey { get; set; }
        public static string Encryptkey { get; set; }
        public static string Issuer { get; set; }
        public static string Audience { get; set; }
        public static int NotBeforeMinutes { get; set; }
        public static int ExpirationMinutes { get; set; }
    }
}
