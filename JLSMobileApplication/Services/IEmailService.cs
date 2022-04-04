namespace JLSMobileApplication.Services
{
    public interface IEmailService
    {
        string SendEmail(string ToEmail, string Subjet, string Message, string AttachmentPath);
    }
}