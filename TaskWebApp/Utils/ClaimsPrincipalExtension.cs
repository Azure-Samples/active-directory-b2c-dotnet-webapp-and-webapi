using Microsoft.Identity.Client;
using System.Security.Claims;

namespace TaskWebApp.Utils
{
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Get the Account identifier for an MSAL.NET account from a ClaimsPrincipal
        /// </summary>
        /// <param name="claimsPrincipal">Claims principal</param>
        /// <returns>A string corresponding to an account identifier as defined in <see cref="Microsoft.Identity.Client.AccountId.Identifier"/></returns>
        public static string GetMsalAccountId(this ClaimsPrincipal claimsPrincipal)
        {
			string userObjectId = GetObjectId(claimsPrincipal);
			string tenantId = "775527ff-9a37-4307-8b3d-cc311f58d925"; //TODO: FIX THIS

			if (!string.IsNullOrWhiteSpace(userObjectId) && !string.IsNullOrWhiteSpace(tenantId))
			{
				return $"{userObjectId}.{tenantId}";
			}

			return null;
		}

		/// <summary>
		/// Get the unique object ID associated with the claimsPrincipal
		/// </summary>
		/// <param name="claimsPrincipal">Claims principal from which to retrieve the unique object id</param>
		/// <returns>Unique object ID of the identity, or <c>null</c> if it cannot be found</returns>
		public static string GetObjectId(this ClaimsPrincipal claimsPrincipal)
        {
            var objIdclaim = claimsPrincipal.FindFirst(ClaimConstants.ObjectId);

            if (objIdclaim == null)
            {
                objIdclaim = claimsPrincipal.FindFirst("oid");
            }

            return objIdclaim != null ? objIdclaim.Value : string.Empty;
        }

        /// <summary>
        /// Builds a ClaimsPrincipal from an IAccount
        /// </summary>
        /// <param name="account">The IAccount instance.</param>
        /// <returns>A ClaimsPrincipal built from IAccount</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this IAccount account)
        {
            if (account != null)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimConstants.ObjectId, account.HomeAccountId.ObjectId));
                identity.AddClaim(new Claim(ClaimConstants.TenantId, account.HomeAccountId.TenantId));
                identity.AddClaim(new Claim(ClaimTypes.Upn, account.Username));
                return new ClaimsPrincipal(identity);
            }

            return null;
        }
    }
}