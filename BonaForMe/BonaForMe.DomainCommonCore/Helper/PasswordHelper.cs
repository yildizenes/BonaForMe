using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace BonaForMe.DomainCommonCore.Helper
{
    public class PasswordHelper
    {
        public static string PasswordEncoder(string password)
        {
            var hashedPassword = Crypto.Hash(password, "sha256");
            return hashedPassword;
        }

        public static string GeneratePassword(int passLength = 8)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz@/*-+!.&ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, passLength)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}