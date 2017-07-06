using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskWebApp.Models;

namespace TaskWebApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private String apiEndpoint = Startup.ServiceUrl + "/api/tasks/";

        // GET: Makes a call to the API and retrieves the list of tasks
        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = new string[] { Startup.ReadTasksScope };
                string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
                ConfidentialClientApplication cca = new ConfidentialClientApplication(Startup.ClientId, Startup.Authority, Startup.RedirectUri, new ClientCredential(Startup.ClientSecret), userTokenCache, null);

                var user = cca.Users.FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("The User is NULL.  Please clear your cookies and try again.  Specifically delete cookies for 'login.microsoftonline.com'.  See this GitHub issue for more details: https://github.com/Azure-Samples/active-directory-b2c-dotnet-webapp-and-webapi/issues/9");
                }

                AuthenticationResult result = await cca.AcquireTokenSilentAsync(scope, user, Startup.Authority, false);

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        String responseString = await response.Content.ReadAsStringAsync();
                        JArray tasks = JArray.Parse(responseString);
                        ViewBag.Tasks = tasks;
                        return View();
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error reading to do list: " + ex.Message);
            }
        }

        // POST: Makes a call to the API to store a new task
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                // Retrieve the token with the specified scopes
                string accessToken = null;
                try
                {
                    var scope = new string[] { Startup.WriteTasksScope };
                    string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                    TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
                    ConfidentialClientApplication cca = new ConfidentialClientApplication(Startup.ClientId, Startup.Authority, Startup.RedirectUri, new ClientCredential(Startup.ClientSecret), userTokenCache, null);

                    AuthenticationResult result = await cca.AcquireTokenSilentAsync(scope, cca.Users.FirstOrDefault(), Startup.Authority, false);
                    accessToken = result.AccessToken;
                }
                catch (Exception)
                {
                    //Require interactive signin
                }

                // Set the content
                var httpContent = new[] {new KeyValuePair<string, string>("Text", description)};

                // Create the request
                HttpClient client = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error writing to list: " + ex.Message);
            }
        }

        // DELETE: Makes a call to the API to delete an existing task
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = new string[] { Startup.WriteTasksScope };
                string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, this.HttpContext).GetMsalCacheInstance();
                ConfidentialClientApplication cca = new ConfidentialClientApplication(Startup.ClientId, Startup.Authority, Startup.RedirectUri, new ClientCredential(Startup.ClientSecret), userTokenCache, null);

                AuthenticationResult result = await cca.AcquireTokenSilentAsync(scope, cca.Users.FirstOrDefault(), Startup.Authority, false);


                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint + id);

                // Add token to the Authorization header and send the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken); 
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return ErrorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return ErrorAction("Error deleting from list: " + ex.Message);
            }
        }

        /*
         * Helper function for returning an error message
         */
        private ActionResult ErrorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

    }
}