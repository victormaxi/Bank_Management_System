using _Core.Utility;

namespace _Core.Interfaces
{
    public interface IEmailSender
    {
        void SendEmailAsync(Message message);
        void SendConfirmEmailAsync(Message message);
        void SendAttachmentEmailAsync(Message message);
    }
}
