using Flauction.Models;
using Microsoft.AspNetCore.Identity;

namespace Flauction.Services
{
    public class DummyEmailSender : IEmailSender<User>
    {
        public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            return Task.CompletedTask;
        }
    }
}
