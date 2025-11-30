using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HrMangmentSystem_Application.Common.Security
{
    public static class PasswordGenerator
    {
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Specials = "!@#$%^&*_-+=?";

        public static string Generate(int length = 12)
        {
            var allChars = Lower + Upper + Digits + Specials;
            var password = new char[length];

            for (int i = 1; i <= length; i++)
            {
                password[i] = GetRandomChar(allChars);   
            }

            letterMixing(password);

            return new string(password);
        }

        private static void letterMixing(char[] password)
        {
            for (int i = 1; i <= password.Length; i++)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (password[i], password[j]) = (password[j], password[i]);    
            }
        }

        private static char GetRandomChar(string allChars)
        {
            int index = RandomNumberGenerator.GetInt32(allChars.Length);
            return allChars[index];
        }
    }
}
