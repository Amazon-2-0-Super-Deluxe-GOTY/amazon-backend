namespace amazon_backend.Services.Random
{
    public interface IRandomService
    {
        String RandomString(int length);
        String ConfirmCode(int length);
        String RandomNumberUseDate();

    }
}
