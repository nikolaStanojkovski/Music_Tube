using MusicTube.Domain.Domain;
using MusicTube.Repository.Interface;
using MusicTube.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Implementation
{
    public class BackgroundEmailSender : IBackgroundEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _mailRepository;

        public BackgroundEmailSender(IEmailService emailService,
             IRepository<EmailMessage> mailRepository)
        {
            this._emailService = emailService;
            this._mailRepository = mailRepository;
        }

        public async Task DoWork()
        {
            List<EmailMessage> messages = this._mailRepository.ReadAll()
                .Where(z => !z.Status).ToList();

            await _emailService.SendEmailAsync(messages);

            if (messages != null && messages.Count != 0)
                UpdateStatus(messages); // Update the status to true for messages sent
        }

        private void UpdateStatus(List<EmailMessage> emails)
        {
            foreach (var message in emails)
            {
                message.Status = true;
            }
            _mailRepository.UpdateAll(emails);
        }
    }
}
