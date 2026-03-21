using AMB.Application.Common;
using AMB.Application.Interfaces.Services;
using Resend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Infra.Notifications
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly string _templatePath;

        public ResendEmailService(IResend resend)
        {
            _resend = resend;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "Notifications", "EmailTemplates");
        }

        public async Task SendEmailAsync(string to, string subject, string templateName, object model)
        {
            string filePath = Path.Combine(_templatePath, $"{templateName}.hbs");
            string templateContent = await File.ReadAllTextAsync(filePath);

            string htmlBody = NotificationTemplateEngine.Render(templateContent, model);

            var message = new EmailMessage
            {
                From = "no-reply@ambrosiahq.com",
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
