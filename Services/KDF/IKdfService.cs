using amazon_backend.Services.Hash;

namespace amazon_backend.Services.KDF
{
    public interface IKdfService
    {
        string GetDerivedKey(String password, String salt);
    }
}
