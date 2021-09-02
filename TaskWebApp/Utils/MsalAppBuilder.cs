/************************************************************************************************
The MIT License (MIT)

Copyright (c) 2015 Microsoft Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
***********************************************************************************************/

using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TaskWebApp.Utils
{



	public static class MsalAppBuilder
	{
		/// <summary>
		/// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
		/// </summary>
		/// <returns></returns>
		public static IConfidentialClientApplication BuildConfidentialClientApplication()
		{
			return BuildConfidentialClientApplication(ClaimsPrincipal.Current);
		}

        public class MyHttpClientFactory : IMsalHttpClientFactory
        {
            private static readonly HttpClient s_httpClient;

            static MyHttpClientFactory()
            {
                var webProxy = new WebProxy(
                    new Uri(ConfigurationManager.AppSettings["ProxyServerUrl"]),
                    BypassOnLocal: false);

                webProxy.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ProxyUsername"], ConfigurationManager.AppSettings["ProxyPassword"], ConfigurationManager.AppSettings["ProxyDomain"]);                

                var proxyHttpClientHandler = new HttpClientHandler
                {
                    Proxy = webProxy,
                    UseProxy = true,
                };

                s_httpClient = new HttpClient(proxyHttpClientHandler);

            }

            public HttpClient GetHttpClient()
            {
                return s_httpClient;
            }
        }

        private static IMsalHttpClientFactory httpClientFactory = new MyHttpClientFactory();        

        /// <summary>
        /// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
        /// </summary>
        /// <param name="currentUser">The current ClaimsPrincipal</param>
        public static IConfidentialClientApplication BuildConfidentialClientApplication(ClaimsPrincipal currentUser)
		{
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(Globals.ClientId)
                  .WithClientSecret(Globals.ClientSecret)
                  .WithRedirectUri(Globals.RedirectUri)
                  .WithB2CAuthority(Globals.B2CAuthority)
                  .WithHttpClientFactory(httpClientFactory)
				  .Build();

			MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache, currentUser ?? ClaimsPrincipal.Current);
			return clientapp;
		}

		/// <summary>
		/// Common method to remove the cached tokens for the currently signed in user
		/// </summary>
		/// <returns></returns>
		public static async Task ClearUserTokenCache()
		{
			IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(Globals.ClientId)
				.WithB2CAuthority(Globals.B2CAuthority)
				.WithClientSecret(Globals.ClientSecret)
				.WithRedirectUri(Globals.RedirectUri)
				.Build();

			// We only clear the user's tokens.
			MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache);
			var userAccounts = await clientapp.GetAccountsAsync();

			foreach (var account in userAccounts)
			{
				//Remove the users from the MSAL's internal cache
				await clientapp.RemoveAsync(account);
			}
            userTokenCache.Clear();

        }
    }
}