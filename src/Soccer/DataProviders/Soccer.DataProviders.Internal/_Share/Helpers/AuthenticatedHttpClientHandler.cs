using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Soccer.Core._Shared.Helpers;
using Soccer.Core.Shared.Models;

namespace Soccer.DataProviders.Internal.Share.Helpers
{
    public interface IAuthenticateApi
    {
        [Post("/authenticate/generateToken")]
        Task<string> Authenticate([Body]AuthenticateInfo authenticateInfo);
    }

    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private static string token;
        private const string Scheme = "Bearer";
        private const string AuthenticateName = "EventListener";
        private readonly Func<AuthenticateInfo, Task<string>> getToken;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly string encryptKey;

        public AuthenticatedHttpClientHandler(
            Func<AuthenticateInfo, Task<string>> getToken, 
            ICryptographyHelper cryptographyHelper,
            string encryptKey)
        {
            if (getToken == null)
            {
                throw new ArgumentNullException(nameof(getToken));
            }
            this.cryptographyHelper = cryptographyHelper;
            this.getToken = getToken;
            this.encryptKey = encryptKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
#pragma warning disable S2696 // Instance members should not write to "static" fields
                token = await getToken(new AuthenticateInfo 
                { 
                    UserId = AuthenticateName, 
                    EncryptedInfo = cryptographyHelper.Encrypt(AuthenticateName, encryptKey) 
                }).ConfigureAwait(false);
#pragma warning restore S2696 // Instance members should not write to "static" fields
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}