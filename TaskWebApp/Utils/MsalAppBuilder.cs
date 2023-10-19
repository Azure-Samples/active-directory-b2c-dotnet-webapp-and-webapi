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

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace TaskWebApp.Utils
{
    public static class MsalAppBuilder
    {
        /// <summary>
        /// Shared method to create an IConfidentialClientApplication from configuration and attach the application's token cache implementation
        /// </summary>
        public static IConfidentialClientApplication BuildConfidentialClientApplication()
        {
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(Globals.ClientId)
                  .WithClientSecret(Globals.ClientSecret)
                  .WithRedirectUri(Globals.RedirectUri)
                  .WithB2CAuthority(Globals.B2CAuthority)
                  .Build();

            // Important: for simplicity, this sample showcases an in-memory cache
            // For eviction policies and for using a distribued cache see:
            // https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-net-token-cache-serialization?tabs=aspnet

            //Add an in-memory token cache with options
            clientapp.AddInMemoryTokenCache();

            return clientapp;
        }       
    }
}