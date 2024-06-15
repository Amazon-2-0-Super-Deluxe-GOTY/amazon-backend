namespace amazon_backend.Services.Hash
{
    public interface IHashFunction
    {
        string Hash(string password, string salt = null);
        bool VerifyPassword(string password, string salt, string hashedPassword);
    }
}
