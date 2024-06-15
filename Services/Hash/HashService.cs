namespace amazon_backend.Services.Hash
{
    public interface IHashService<T> where T : IHashFunction, new()
    {
        string HashPassword(string password, string salt = null);
        bool VerifyPassword(string password, string salt, string hashedPassword);
    }
    public class HashService<T> : IHashService<T> where T : IHashFunction, new()
    {
        private readonly T _hashFunc;
        public HashService()
        {
            _hashFunc = new T();
        }
        public string HashPassword(string password, string salt = null)
        {
            return _hashFunc.Hash(password, salt);
        }

        public bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            return _hashFunc.VerifyPassword(password, salt, hashedPassword);
        }
    }
}
