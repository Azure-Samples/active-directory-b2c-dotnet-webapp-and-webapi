using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace TaskWebApp.Utils
{
    public static class MsalAppBuilder
    {
        public static IConfidentialClientApplication BuildConfidentialClientApplication()
        {
            IConfidentialClientApplication clientapp = ConfidentialClientApplicationBuilder.Create(Globals.ClientId)
                .WithB2CAuthority(Globals.B2CAuthority)
                .WithClientSecret(Globals.ClientSecret)
                .WithRedirectUri(Globals.RedirectUri)
                .Build();

            MSALPerUserMemoryTokenCache userTokenCache = new MSALPerUserMemoryTokenCache(clientapp.UserTokenCache);
            return clientapp;
        }

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

			userTokenCache.Clear();

			foreach (var account in userAccounts)
			{
				await clientapp.RemoveAsync(account);
			}
		}
    }
}