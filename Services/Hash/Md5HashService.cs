using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace amazon_backend.Services.Hash
{
    public class Md5HashService : IHashService
    {
        public string Hash(string text)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
             return
                Convert.ToHexString(
                    md5.ComputeHash(
                        System.Text.Encoding.UTF8.GetBytes(text)));
        }
    }
}
