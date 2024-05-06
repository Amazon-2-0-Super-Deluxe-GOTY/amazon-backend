using amazon_backend.Services.Hash;

namespace amazon_backend.Services.KDF
{
    public class HashKdfService: IKdfService
    {
        private readonly IHashService _hashService;

        public HashKdfService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public String GetDerivedKey(string password, String salt)
        {
            return _hashService.Hash(salt + password);
        }
    }
}
