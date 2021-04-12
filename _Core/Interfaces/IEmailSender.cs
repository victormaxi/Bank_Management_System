using _Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace _Core.Interfaces
{
    public interface IEmailSender
    {
        void SendEmailAsync(Message message);
        void SendConfirmEmailAsync(Message message);
        void SendAttachmentEmailAsync(Message message);
    }
}
