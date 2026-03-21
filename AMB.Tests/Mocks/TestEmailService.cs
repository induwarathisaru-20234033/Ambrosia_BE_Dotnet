using AMB.Application.Interfaces.Services;

namespace AMB.Tests.Mocks
{
    internal sealed class TestEmailService : IEmailService
    {
        public string? LastTo { get; private set; }
        public string? LastSubject { get; private set; }
        public string? LastTemplateName { get; private set; }
        public object? LastModel { get; private set; }

        public Task SendEmailAsync(string to, string subject, string templateName, object model)
        {
            LastTo = to;
            LastSubject = subject;
            LastTemplateName = templateName;
            LastModel = model;
            return Task.CompletedTask;
        }
    }
}
