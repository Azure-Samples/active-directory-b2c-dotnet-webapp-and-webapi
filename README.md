---
services: active-directory-b2c
platforms: dotnet
author: dstrockis
---

# Azure AD B2C: Call a .NET web API from a .NET web app

This sample has a solution file that contains two projects: `TaskWebApp` and `TaskService`. `TaskWebApp` is a "To-do" MVC web application that the user interacts with. `TaskService` is the app's back-end web API that stores each user's to-do list. You can use this sample to quickly get started with building a .NET web app, web api, and calling the api from the web app.

The sample also covers:

* Calling an OpenID Connect identity provider (Azure AD B2C)
* Acquiring a token from Azure AD B2C using MSAL


See our detailed documentation on:

* [How to build a .NET web app using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-dotnet-susi)
* [How to build a .NET web API secured using Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-api-dotnet)
* [How to call a .NET web api using a .NET web app](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-devquickstarts-web-api-dotnet)

## Getting started

Our sample is configured to use the policies and client ID of our demo tenant. To use our sample with your own configuration:

1. [Create an Azure AD B2C tenant](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-get-started).
2. [Register a web api](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration).
3. [Register a web app](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration).
4. [Set up policies](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-reference-policies).
5. [Grant the web app permissions to use the web api](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-access-tokens).

**Note: The client application and web API must use the same Azure AD B2C directory.**

### Update the Azure AD B2C configuration

Open `web.config` in the `TaskService` project and replace the values for

* `ida:Tenant` with your tenant name
* `ida:ClientId` with your web api application ID
* `ida:SignUpSignInPolicyId` with your "Sign-up or Sign-in" policy name


Open `web.config` in the `TaskWebApp` project and replace the values for

* `ida:Tenant` with your tenant name
* `ida:ClientId` with your web app application ID
* `ida:ClientSecret` with your web app secret key
* `ida:SignUpSignInPolicyId` with your "Sign-up or Sign-in" policy name
* `ida:EditProfilePolicyId` with your "Edit Profile" policy name
* `ida:ResetPasswordPolicyId` with your "Reset Password" policy name

### Run the sample

Build and run both the apps. Sign up and sign in, and create tasks for the signed-in user. Sign out and sign in as a different user. Create tasks for that user. Notice how the tasks are stored per-user on the API, because the API extracts the user's identity from the token it receives. Also try playing with the scopes. Remove the permission to "write" and then try adding a task. Just make sure to sign out each time you change the scope.

## Next Steps

Customize your user experience further by supporting more identity providers.  Checkout the docs belows to learn how to add additional providers:

[Microsoft](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-msa-app)

[Facebook](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-fb-app)

[Google](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-goog-app)

[Amazon](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-amzn-app)

[LinkedIn](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-setup-li-app)


## Questions & Issues

Please file any questions or problems with the sample as a github issue. You can also post on [StackOverflow](https://stackoverflow.com/questions/tagged/azure-ad-b2c) with the tag `azure-ad-b2c`.
