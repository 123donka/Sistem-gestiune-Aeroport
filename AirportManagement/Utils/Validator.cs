using System.Text.RegularExpressions;

namespace AirportManagement.Utils
{
    public static class Validator
    {
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            var re = new Regex(@"^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$");
            return re.IsMatch(email);
        }

        public static bool IsStrongPassword(string pwd)
        {
            if (string.IsNullOrEmpty(pwd) || pwd.Length < 6) return false;
            return true;
        }
    }
}
