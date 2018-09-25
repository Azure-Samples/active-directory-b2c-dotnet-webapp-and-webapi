using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;
using System.Collections.Generic;

namespace TaskService.App_Start
{
    // This class is necessary because the OAuthBearer Middleware does not leverage
    // the OpenID Connect metadata endpoint exposed by the STS by default.
    public class OpenIdConnectCachingSecurityTokenProvider : IIssuerSecurityKeyProvider
    {
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;
        public OpenIdConnectCachingSecurityTokenProvider(string metadataEndpoint)
        {
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint, new OpenIdConnectConfigurationRetriever());
            _configManager.GetConfigurationAsync().Wait();
        }

        /// <summary>
        /// Gets the issuer the credentials are for.
        /// </summary>
        /// <value>
        /// The issuer the credentials are for.
        /// </value>
        public string Issuer => _configManager.GetConfigurationAsync().Result.Issuer;

        /// <summary>
        /// Gets all known security keys.
        /// </summary>
        /// <value>
        /// All known security keys.
        /// </value>
        public IEnumerable<SecurityKey> SecurityKeys => _configManager.GetConfigurationAsync().Result.SigningKeys;
    }
}
