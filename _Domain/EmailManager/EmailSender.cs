using _Core.Interfaces;
using _Core.Utility;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _Domain.EmailManager
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly GmailKey _gmailKey;

        public EmailSender(EmailConfiguration emailConfig, GmailKey gmailKey)
        {
            _emailConfig = emailConfig;
            _gmailKey = gmailKey;
        }
        public async void SendConfirmEmailAsync(Message message)
        {
            try
            {
                var emailMessage = ConfirmEmailMessage(message);
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
            { Text = string.Format(@"<h2> style = 'color:red;'> {0} </h2>", message.Content) };

            return emailMessage;
        }
        private MimeMessage CreateEmailMessageWithAttachment(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
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

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port_SSL, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);


                    await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
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

        private async Task Oauth(MimeMessage mimeMessage)
        {
            const string GMailAccount = "aqvarproduction@gmail.com";

            var clientSecrets = new ClientSecrets
            {
                ClientId = _gmailKey.Authentication_Google_ClientId,
                ClientSecret = _gmailKey.Authentication_Google_ClientSecret
            };

            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                // Cache tokens in ~/.local/share/google-filedatastore/CredentialCacheFolder on Linux/Mac
                DataStore = new FileDataStore("CredentialCacheFolder", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = clientSecrets
            });

            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);
            var credential = await authCode.AuthorizeAsync(GMailAccount, CancellationToken.None);

            if (authCode.ShouldRequestAuthorizationCode(credential.Token))
                await credential.RefreshTokenAsync(CancellationToken.None);

            var oauth2 = new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken);

            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(oauth2);
                //await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }


        }

        public void SendEmailAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
