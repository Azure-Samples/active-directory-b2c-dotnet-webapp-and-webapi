---
services: active-directory-b2c
platforms: dotnet
author: dstrockis
---

# Azure AD B2C: Call a ASP.NET Web API from a ASP.NET Web App

This sample contains a solution file that contains two projects: `TaskWebApp` and `TaskService`. 

- `TaskWebApp` is a "To-do" ASP.NET MVC web application where the users enters or updates their to-do items.  These CRUD operations are performed by a backend web API. The web app displays the information returned from the ASP.NET Web API.
- `TaskService` is the backend ASP.NET API that manages and stores each user's to-do list. 

The sample covers the following: 

* Calling an OpenID Connect identity provider (Azure AD B2C)
* Acquiring a token from Azure AD B2C using MSAL

## How To Run This Sample

There are two ways to run this sample:

1. **Using the demo environment** - The sample is already configured to use a demo environment and can be run by downloading this repository and running the app on your machine. Follow the steps listed below in the section [Using the demo environment](#Using the demo environment)
2. **Using your own Azure AD B2C tenant** - Once you have the sample running locally using the demo tenant, you can configure the sample to use your own Azure AD B2C tenant instead. Follow the steps listed below in the section [Using your own Azure AD B2C tenant](#Using your own Azure AD B2C Tenant)

## Using the demo environment

This sample demonstrates how you can sign in or sign up for an account at "Wingtip Toys" (the demo environment for this sample) using a ASP.NET MVC Web Application. 

Once singed in, you can create and edit your todo items. 

### Step 1: Clone or download this repository

From your shell or command line:

```
git clone https://github.com/Azure-Samples/active-directory-b2c-dotnet-desktop.git
```

### Step 2: Run the project

Open the `B2C-WebAPI-DotNet.sln` in Visual Studio. 

You will need to run both the `TaskWebApp` and `TaskService` projects at the same time. 

1. In Solution Explorer, right-click on the solution and open the **Common Properties - Startup Project** window. 
2. Select **Multiple startup projects**.
3. Change the **Action** for both projects from None to **Start** as shown in the image below.

TODO: add image

The sample demonstrates the following functionality once signed-in: 

1. Click your **``<Display Name>``** in upper right corner to edit your profile or reset your password. 
2. Click **Claims** to view the claims associated with the signed-in user's id token. 
3. Click **Todo** to create and view your todo items. These CRUD operations are performed by calling the corresponding Web API running in the solution.
4. Sign out and sign in as a different user. Create tasks for this second user. Notice how the tasks are stored per-user on the API, because the API extracts the user's identity from the access token it receives. 

## Using your own Azure AD B2C Tenant



## Next Steps

Customize your user experience further by supporting more identity providers.  Checkout the docs belows to learn how to add additional providers:

[Microsoft](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-msa-app)

[Facebook](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-fb-app)

[Google](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-goog-app)

[Amazon](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-amzn-app)

[LinkedIn](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-li-app)


## Additional information

Additional information regarding this sample can be found in our documentation:

* [How to build a .NET web app using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)
* [How to build a .NET web API secured using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-api-dotnet)
* [How to call a .NET web api using a .NET web app](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-api-dotnet)

## Questions & Issues

Please file any questions or problems with the sample as a github issue. You can also post on [StackOverflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) with the tag `azure-ad-b2c`.


