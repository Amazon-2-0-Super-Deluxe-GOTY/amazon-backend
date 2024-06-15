using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace amazon_backend.Services.Hash
{
    public class Md5HashService : IHashFunction
    {
        public string Hash(string password, string salt = null)
        {
            var res = salt == null ? password : salt + password;
            using var md5 = System.Security.Cryptography.MD5.Create();
            return
               Convert.ToHexString(
                   md5.ComputeHash(
                       System.Text.Encoding.UTF8.GetBytes(res)));
        }


        public bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            string hash = Hash(password, salt);
            return hash == hashedPassword;
        }
    }
}
