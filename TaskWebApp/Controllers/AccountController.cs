﻿using Microsoft.Identity.Client;
using Microsoft.Owin.Security;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TaskWebApp.Utils;

namespace TaskWebApp.Controllers
{
    public class AccountController : Controller
    {
        /*
         *  Called when requesting to sign up or sign in
         */
        public void SignUpSignIn(string redirectUrl)
        {
			redirectUrl = redirectUrl ?? "/";

			// Use the default policy to process the sign up / sign in flow
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });
            return;
        }

        /*
         *  Called when requesting to edit a profile
         */
        public void EditProfile()
        {
            if (Request.IsAuthenticated)
            {
                // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
                HttpContext.GetOwinContext().Set("Policy", Globals.EditProfilePolicyId);

                // Set the page to redirect to after editing the profile
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

                return;
            }

            Response.Redirect("/");
        }

        /*
         *  Called when requesting to reset a password
         */
        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        /*
         *  Called when requesting to sign out
         */
        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                // Remove all tokens from MSAL's cache
                var clientApp= MsalAppBuilder.BuildConfidentialClientApplication();
                string accountId = ClaimsPrincipal.Current.GetB2CMsalAccountIdentifier();
                IAccount account = await clientApp.GetAccountAsync(accountId);
                await clientApp.RemoveAsync(account);

                // Then sign-out from OWIN
				IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
        }
	}
}