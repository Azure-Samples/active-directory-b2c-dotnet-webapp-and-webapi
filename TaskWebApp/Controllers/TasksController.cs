using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskWebApp.Utils;

namespace TaskWebApp.Controllers
{
	[Authorize]
    public class TasksController : Controller
    {
        private String apiEndpoint = Globals.ServiceUrl + "/api/tasks/";

        // GET: Makes a call to the API and retrieves the list of tasks
        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = new string[] { Globals.ReadTasksScope };
                
                IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
                var accounts = await cca.GetAccountsAsync();
				AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();
                
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
                        return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
                }
            }
			catch (MsalUiRequiredException ex)
			{
				/*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
				return new RedirectResult("/Account/SignUpSignIn?redirectUrl=/Tasks");
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
                var scope = new string[] { Globals.WriteTasksScope };

				IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
				var accounts = await cca.GetAccountsAsync();
				AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();
				accessToken = result.AccessToken;
                
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
			catch (MsalUiRequiredException ex)
			{
				/*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
				return new RedirectResult("/Account/SignUpSignIn?redirectUrl=/Tasks");
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
                var scope = new string[] { Globals.WriteTasksScope };

				IConfidentialClientApplication cca = MsalAppBuilder.BuildConfidentialClientApplication();
				var accounts = await cca.GetAccountsAsync();
				AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault()).ExecuteAsync();

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
			catch (MsalUiRequiredException ex)
			{
				/*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
				return new RedirectResult("/Account/SignUpSignIn?redirectUrl=/Tasks");
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