using BCrypt.Net;

namespace amazon_backend.Services.Hash
{
    public class BcryptHashService : IHashFunction
    {
        public string Hash(string password, string salt = null)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            try
            {
                var result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                return result;
            }
            catch(SaltParseException ex)
            {
                return false;
            }
        }
    }
}
