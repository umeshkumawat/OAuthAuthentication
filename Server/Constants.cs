using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Constants
    {
        public const string Audience = "http://localhost:50059";
        public const string Issuer = Audience;
        public const string Secret = "not_too_short_secret_otherwise_it_might_errer";
    }
}
