namespace Service.Interface.UtilityServices
{
    public interface IEmailService
    {
        Task SendEmail(string toAddress, string subject, string body);
    }
}
