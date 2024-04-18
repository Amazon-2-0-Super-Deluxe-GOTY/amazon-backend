namespace amazon_backend.Services.Email
{
    public interface IEmailService
    {
        bool Send(string mailTemplate, object model);//nubbubuj
    }
}
