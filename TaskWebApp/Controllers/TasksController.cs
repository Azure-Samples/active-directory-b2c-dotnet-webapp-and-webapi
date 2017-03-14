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

namespace TaskWebApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {

        private String accessToken;
        private String apiEndpoint = Startup.ServiceUrl + "/api/tasks/";

        // GET: Makes a call to the API and retrieves the list of tasks
        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve the token with the specified scopes
                acquireToken(new string[] { Startup.ReadTasksScope });

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

                // Add token to the Authorization header and make the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error reading to do list: " + ex.Message);
            }
        }

        // POST: Makes a call to the API to store a new task
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                // Retrieve the token with the specified scopes
                acquireToken(new string[] { Startup.WriteTasksScope });

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
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error writing to list: " + ex.Message);
            }
        }

        // DELETE: Makes a call to the API to delete an existing task
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                // Retrieve the token with the specified scopes
                acquireToken(new string[] { Startup.WriteTasksScope });

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint + id);

                // Add token to the Authorization header and send the request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); 
                HttpResponseMessage response = await client.SendAsync(request);

                // Handle the response
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error deleting from list: " + ex.Message);
            }
        }

        /*
         * Uses MSAL to retrieve the token from the cache or Azure AD B2C
         */
        private async void acquireToken(String[] scope)
        {
            string userObjectID = ClaimsPrincipal.Current.FindFirst(Startup.ObjectIdElement).Value;
            string authority = String.Format(Startup.AadInstance, Startup.Tenant, Startup.DefaultPolicy);

            ClientCredential credential = new ClientCredential(Startup.ClientSecret);

            // Retrieve the token using the provided scopes
            ConfidentialClientApplication app = new ConfidentialClientApplication(authority, Startup.ClientId, 
                                                Startup.RedirectUri, credential, 
                                                new NaiveSessionCache(userObjectID, this.HttpContext));
            AuthenticationResult result = await app.AcquireTokenSilentAsync(scope);

            accessToken = result.Token;
        }

        /*
         * Helper function for returning an error message
         */
        private async Task<ActionResult> errorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

    }
}