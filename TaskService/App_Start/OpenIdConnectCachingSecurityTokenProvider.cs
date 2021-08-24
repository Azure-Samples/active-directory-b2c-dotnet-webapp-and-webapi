using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Jwt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskService.App_Start
{
    // This class is necessary because the OAuthBearer Middleware does not leverage
    // the OpenID Connect metadata endpoint exposed by the STS by default.
    public class OpenIdConnectCachingSecurityTokenProvider : IIssuerSecurityKeyProvider
    {
        public ConfigurationManager<OpenIdConnectConfiguration> _configManager;
        private string _issuer;
        private IEnumerable<SecurityKey> _keys;
        private readonly string _metadataEndpoint;

        private readonly ReaderWriterLockSlim _synclock = new ReaderWriterLockSlim();

        private static HttpClient _httpClient;

        public class MyProxy : IWebProxy
        {
            public ICredentials Credentials
            {
                //get { return new NetworkCredential("user", "password"); }
                get { return new NetworkCredential(ConfigurationManager.AppSettings["ProxyUsername"], ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]); }
                set { }
            }

            public Uri GetProxy(Uri destination)
            {
                return new Uri(ConfigurationManager.AppSettings["ProxyServerUrl"]);
            }

            public bool IsBypassed(Uri host)
            {
                return false;
            }
        }

        private static void CreateHttpClient()
        {
            var config = new HttpClientHandler
            {
                UseProxy = true,
                Proxy = new MyProxy()
            };

            //then you can simply pass the config to HttpClient
            _httpClient = new HttpClient(config);
        }

        public OpenIdConnectCachingSecurityTokenProvider(string metadataEndpoint)
        {
            if (_httpClient == null)
            {
                // It's best* to create a single HttpClient and reuse it                
                CreateHttpClient();
            }
            _metadataEndpoint = metadataEndpoint;
            //_configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint, new OpenIdConnectConfigurationRetriever());
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint, new OpenIdConnectConfigurationRetriever(), _httpClient);
           

            RetrieveMetadata();
        }

        /// <summary>
        /// Gets the issuer the credentials are for.
        /// </summary>
        /// <value>
        /// The issuer the credentials are for.
        /// </value>
        public string Issuer
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _issuer;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets all known security keys.
        /// </summary>
        /// <value>
        /// All known security keys.
        /// </value>
        public IEnumerable<SecurityKey> SecurityKeys
        {
            get
            {
                RetrieveMetadata();
                _synclock.EnterReadLock();
                try
                {
                    return _keys;
                }
                finally
                {
                    _synclock.ExitReadLock();
                }
            }
        }

        private void RetrieveMetadata()
        {
            _synclock.EnterWriteLock();
            try
            {
                OpenIdConnectConfiguration config = Task.Run(_configManager.GetConfigurationAsync).Result;
                _issuer = config.Issuer;
                _keys = config.SigningKeys;
            }
            finally
            {
                _synclock.ExitWriteLock();
            }
        }
    }
}
