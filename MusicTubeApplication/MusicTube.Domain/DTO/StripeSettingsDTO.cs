using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.DTO
{
    public class StripeSettingsDTO
    {
        public string PublishableKey { get; set; }

        public string SecretKey { get; set; }
    }
}
