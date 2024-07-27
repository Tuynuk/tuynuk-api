using System.Security.Cryptography;
using System.Text;

namespace Tuynuk.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToSHA256Hash(this string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
                return builder.ToString();
            }
        }
    }
}
