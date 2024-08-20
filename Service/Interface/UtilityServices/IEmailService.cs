namespace Service.Interface.UtilityServices
{
    public interface IEmailService
    {
        Task SendEmail(List<string> toAddresses, string subject, string body);
    }
}
