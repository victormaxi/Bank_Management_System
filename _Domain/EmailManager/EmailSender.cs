using _Core.Interfaces;
using _Core.Utility;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Domain.EmailManager
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async void SendConfirmEmailAsync(Message message)
        {
            try
            {
                var emailMessage =  ConfirmEmailMessage(message);
                await SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async void SendAttachmentEmailAsync(Message message)
        {
            try
            {
                var emailMessage = CreateEmailMessageWithAttachment(message);
               await SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private MimeMessage ConfirmEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            { Text = string.Format("<h2> style = 'color:red;'> {0} </h2>", message.Content) };

            return emailMessage; 
        }
        private MimeMessage CreateEmailMessageWithAttachment(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            if(message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach(var attachment in message.Attachments)
                {
                    using(var ms = new MemoryStream())
                    {
                        attachment.CopyToAsync(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private async Task SendAsync (MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                     client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
                    

                   await client.SendAsync(mailMessage);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                   await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public void SendEmailAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
