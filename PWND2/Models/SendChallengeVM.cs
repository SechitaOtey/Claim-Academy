using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWND2.Models
{
    public class SendChallengeVM
    {
        public int challengeId  { get; set; }
        public IEnumerable<string> KIdIds { get; set; }

    }
}