using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Api
{

    // Authorization fail होने पर हमे challenge किया जाता है और challegne मिलने पर ये वाला handler execute होता है।
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // क्योंकि API Token से ही authorize होगी, इसलिए हम explicit authentication requests को fail कर देंगे।
            return Task.FromResult(AuthenticateResult.Fail("Authentication Failed"));
        }
    }
}
