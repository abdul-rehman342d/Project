using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using PowerfulPal.Neeo.DashboardAPI.Utilities;

namespace PowerfulPal.Neeo.DashboardAPI.Filter
{
    public class UserAuthenticationAttribute: Attribute, IAuthenticationFilter
    {
        private const string Scheme = "ndp";
        public Task AuthenticateAsync(HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            
            var request = context.Request;
            if (request.Headers.Authorization == null)
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }
            if (request.Headers.Authorization.Scheme != Scheme)
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }
            var authKey = request.Headers.Authorization.Parameter;
            
            if (!AppSession.Validate(authKey))
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }
    }
}