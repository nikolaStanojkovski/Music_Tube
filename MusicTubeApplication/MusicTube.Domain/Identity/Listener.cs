using MusicTube.Domain.Domain.Subdomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTube.Domain.Identity
{
    public class Listener : MusicTubeUser
    {
        public List<Review> Reviews { get; set; }
    }
}
