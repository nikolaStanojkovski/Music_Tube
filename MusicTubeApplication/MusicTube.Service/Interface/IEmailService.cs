using MusicTube.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Service.Interface
{
    public interface IEmailService
    {
        public Task SendEmailAsync(List<EmailMessage> emails);
    }
}
