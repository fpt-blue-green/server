
using Repositories;
using Service;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private static ConfigManager _configManager = new ConfigManager();
    private static ISystemSettingRepository _systemSettingRepository = new SystemSettingRepository();

    public Dictionary<string, string> ParseEmailSettings(string inputString)
    {
        var settingsDictionary = new Dictionary<string, string>();
        var settings = inputString.Split(',');

        foreach (var setting in settings)
        {
            var keyValue = setting.Split(':');
            if (keyValue.Length == 2)
            {
                settingsDictionary[keyValue[0]] = keyValue[1];
            }
        }

        return settingsDictionary;
    }

    public async Task SendEmail(List<string> toAddresses, string subject, string body)
    {
        var emailSetting = await _systemSettingRepository.GetSystemSetting(_configManager.EmailConfig);
        var emailSettings = ParseEmailSettings(emailSetting.KeyValue!);

        var fromAddress = emailSettings["FromAddress"];
        var smtpServer = emailSettings["SmtpServer"];
        var smtpPort = int.Parse(emailSettings["SmtpPort"]);
        var smtpUsername = emailSettings["SmtpUsername"];
        var smtpPassword = emailSettings["SmtpPassword"];
        var fromName = emailSettings["FromName"];

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromAddress, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var toAddress in toAddresses)
        {
            mailMessage.To.Add(toAddress);
        }

        using (var client = new SmtpClient(smtpServer, smtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
            client.EnableSsl = true;
            await client.SendMailAsync(mailMessage);
        }
    }

}