<<<<<<< .mine
namespace Service.Interface.UtilityServices
{
    public interface IEmailService
    {
        Task SendEmail(List<string> toAddresses, string subject, string body);
    }
}
=======
namespace Service.Interface.UtilityServices
{
    public interface IEmailService
    {
        Task SendEmail(string toAddress, string subject, string body);
    }
}
>>>>>>> .theirs
