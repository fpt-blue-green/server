namespace Service.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string toAddress, string subject, string body);
    }
}
