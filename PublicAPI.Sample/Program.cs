// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Generated.Api;
    using Generated.Invokers;
    using Generated.Models.Accounts.State;

    using Models.AuthorizationServer;
    using Models.ResourceServer;
    using Models.ResourceServer.Accounts;
    using Models.ResourceServer.Accounts.Address;
    using Models.ResourceServer.Accounts.Company;
    using Models.ResourceServer.Accounts.Contacts;
    using Models.ResourceServer.Accounts.Limits;
    using Models.ResourceServer.Accounts.Msa;
    using Models.ResourceServer.Accounts.Payment;
    using Models.ResourceServer.Accounts.Plans;

    using Newtonsoft.Json;

    using Thinktecture.IdentityModel.Client;

    /// <summary>
    /// The entry point.
    /// </summary>
    public static class Program
    {
        #region Settings

        /// <summary>
        /// The endpoint address base.
        /// </summary>
        /// <remarks>
        /// The common part of endpoint addresses both for authorization server and resource server.
        /// </remarks>
        private const string EndpointAddressBase = "https://cp.serverdata.net/webservices";

        /// <summary>
        /// The authorization server endpoint address.
        /// </summary>
        /// <remarks>
        /// This one is implemented with 'grant_type' = 'password' and has version 'v1'.
        /// </remarks>
        private const string AuthorizationServerEndpointAddress = EndpointAddressBase + "/auth/v1/token";

        /// <summary>
        /// The resource server endpoint address base.
        /// </summary>
        /// <remarks>
        /// This one is used by Swagger Code Generator classes due to its specific implementation.
        /// </remarks>
        private const string ResourceServerEndpointAddressBase = EndpointAddressBase + "/restapi";

        /// <summary>
        /// The resource server endpoint address.
        /// </summary>
        /// <remarks>
        /// This one is implemented for 'Bearer' token authorization and has version 'v1'.
        /// </remarks>
        private const string ResourceServerEndpointAddress = ResourceServerEndpointAddressBase + "/v1/api";

        /// <summary>
        /// The client id.
        /// </summary>
        /// <remarks>
        /// Please provide your Intermedia Partner Public API client id.
        /// </remarks>
        private const string ClientID = "your_client_id_here";

        /// <summary>
        /// The client secret.
        /// </summary>
        /// <remarks>
        /// Please provide your Intermedia Partner Public API client secret.
        /// </remarks>
        private const string ClientSecret = "your_client_secret_here";

        /// <summary>
        /// The contact login.
        /// </summary>
        /// <remarks>
        /// Please provide your Intermedia Partner Public API contact login.
        /// </remarks>
        private const string ContactLogin = "your_contact_login_here";

        /// <summary>
        /// The contact password.
        /// </summary>
        /// <remarks>
        /// Please provide your Intermedia Partner Public API contact password.
        /// </remarks>
        private const string ContactPassword = "your_contact_password_here";

        /// <summary>
        /// The HTTP timeout.
        /// </summary>
        /// <remarks>
        /// The timeout value is defined in minutes only due to internal development environment specific.
        /// Production system mustn't have such delays.
        /// </remarks>
        private static readonly TimeSpan HttpTimeout = TimeSpan.FromMinutes(5);

        #endregion

        #region Main

        /// <summary>
        /// The main handler.
        /// </summary>
        public static void Main()
        {
            try
            {
                // entire async workflow is wrapped into separate handler
                MainAsync().Wait();
                Write("Demo completed.");
            }
            catch (Exception ex)
            {
                // usually relevant exception should be extracted from AggregateException
                // we try to extract it and print out
                Write(ExtractException(ex).ToString());
                Write("\r\n\r\n");
            }

            Write("Press any key to quit.");
            Console.ReadKey();
        }

        /// <summary>
        /// The main handler (async).
        /// </summary>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task MainAsync()
        {
            // four sequential demo steps
            await HandleTokensUsingHttpClient();
            await HandleTokensUsingOAuth2Client();
            var accountID = await HandleResourcesUsingHttpClient();
            await HandleResourcesUsingApiClient(accountID);
        }

        /// <summary>
        /// Extracts meaningful exception.
        /// </summary>
        /// <param name="source">
        /// The source exception.
        /// </param>
        /// <returns>
        /// The relevant exception.
        /// </returns>
        private static Exception ExtractException(Exception source)
        {
            // other than AggregateException should be handled in a usual way
            if (!(source is AggregateException) || source.InnerException == null)
            {
                return source;
            }

            // the first meaningful thing should be here
            var innerException = source.InnerException;

            // HTTP client timeout case - it can be work-arounded in a special way
            var taskCanceledException = innerException as TaskCanceledException;
            if (taskCanceledException != null && 
                !taskCanceledException.CancellationToken.IsCancellationRequested)
            {
                return new Exception("Request timeout");
            }

            return innerException;
        }

        #endregion

        #region Handle token using HTTP client

        /// <summary>
        /// Demonstrates token handling using HTTP client.
        /// </summary>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task HandleTokensUsingHttpClient()
        {
            // create a new token for the client
            // usually you need to have only one of these per client consumer
            // please consider enabled token limit / rotation:
            // you can occasionally disable one of existent token of your client
            Write("1. Demonstrating token handling using HTTP client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingHttpClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");

            // refreshes an existent token
            // you need to do this only when access token has expired
            // please consider that refresh token can also expire if not used for a long time
            // also refresh token can be intentionally revoked
            // if so then you need to create a new token
            Write("Refreshing a token...");
            token = await RefreshTokenUsingHttpClientAsync(token.RefreshToken);
            Write($"The token was refreshed:\r\n{Dump(token)}\r\n");

            // revokes an existent token
            // usually you don't need to do this, the cases are:
            // 1. a leaked security token
            // 2. you know that related token won't be used (refreshed) anymore
            Write("Revoking a token...");
            await RevokeTokenUsingHttpClientAsync(token.RefreshToken);
            Write("The token was revoked.\r\n\r\n");
        }

        /// <summary>
        /// Creates token using HTTP client.
        /// </summary>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CreateTokenUsingHttpClientAsync()
        {
            // consider https://tools.ietf.org/html/rfc6749#section-4.3
            return await CallTokenUsingHttpClientAsync(new Dictionary<string, string>
            {
                // resource owner password credentials flow
                { "grant_type", "password" },

                // authentication part, your client credentials
                { "client_id", ClientID },
                { "client_secret", ClientSecret },

                // authorication part, your contact credentials
                { "username", ContactLogin },
                { "password", ContactPassword }
            });
        }

        /// <summary>
        /// Refreshes token using HTTP client.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token.
        /// </param>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> RefreshTokenUsingHttpClientAsync(string refreshToken)
        {
            // consider https://tools.ietf.org/html/rfc6749#section-6
            return await CallTokenUsingHttpClientAsync(new Dictionary<string, string>
            {
                // refreshing an token flow
                { "grant_type", "refresh_token" },

                // you still need to authenticate
                { "client_id", ClientID },
                { "client_secret", ClientSecret },

                // token to refresh
                { "refresh_token", refreshToken }
            });
        }

        /// <summary>
        /// Revokes token using HTTP client.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task RevokeTokenUsingHttpClientAsync(string refreshToken)
        {
            // please note it's a custom extension
            // consider https://tools.ietf.org/html/rfc6749#section-4.5
            await CallUsingHttpClientAsync(new Dictionary<string, string>
            {
                // revoking an token flow
                { "grant_type", "revoke_token" },

                // you still need to authenticate
                { "client_id", ClientID },
                { "client_secret", ClientSecret },

                // token to revoke
                { "refresh_token", refreshToken }
            });
        }

        /// <summary>
        /// Calls token using HTTP client.
        /// </summary>
        /// <param name="requestParameters">
        /// The request parameters.
        /// </param>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CallTokenUsingHttpClientAsync(
            IDictionary<string, string> requestParameters)
        {
            // it's a wrapper for 'password' and 'refresh_token' grant types
            // which return new token within content and we must parse it
            var response = await CallUsingHttpClientAsync(requestParameters);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenModel>(responseJson);
        }

        /// <summary>
        /// Calls using HTTP client.
        /// </summary>
        /// <param name="requestParameters">
        /// The request parameters.
        /// </param>
        /// <returns>
        /// The response.
        /// </returns>
        private static async Task<HttpResponseMessage> CallUsingHttpClientAsync(
            IDictionary<string, string> requestParameters)
        {
            // call an Authorization Server endpoint using the HttpClient
            using (var client = new HttpClient { Timeout = HttpTimeout })
            {
                // all Authorization Server calls must be done using 'POST' HTTP verb
                // request content must be provided in 'application/x-www-form-urlencoded' MIME type
                // please note that you can authenticate (pass client_id and client_secret) using Basic Authentication
                // consider https://tools.ietf.org/html/rfc2617
                var response = await client.PostAsync(
                    AuthorizationServerEndpointAddress,
                    new FormUrlEncodedContent(requestParameters));
                response.EnsureSuccessStatusCode();

                // 'password' and 'refresh_token' grant types return token within content
                // 'revoke_token' grant type returns 204 No Content
                if (response.Content != null)
                {
                    await response.Content.LoadIntoBufferAsync();
                }

                return response;
            }
        }

        #endregion

        #region Handle token using OAUTH2 client

        /// <summary>
        /// Demonstrates token handling using OAUTH2 client.
        /// </summary>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task HandleTokensUsingOAuth2Client()
        {
            // the workflow is absolutely the same as in demo #1
            // the only difference is that we use Thinktecture.IdentityModel Client
            // consider https://github.com/IdentityServer
            Write("2. Demonstrating token handling using OAUTH2 client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingOAuth2ClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");

            Write("Refreshing a token...");
            token = await RefreshTokenUsingOAuth2ClientAsync(token.RefreshToken);
            Write($"The token was refreshed:\r\n{Dump(token)}\r\n");

            Write("Revoking a token...");
            await RevokeTokenUsingOAuth2ClientAsync(token.RefreshToken);
            Write("The token was revoked.\r\n\r\n");
        }

        /// <summary>
        /// Creates token using OAUTH2 client.
        /// </summary>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CreateTokenUsingOAuth2ClientAsync()
        {
            // a special method to call the endpoint with a 'grant_type' = 'password'
            return await CallTokenUsingOAuth2ClientAsync(client => 
                client.RequestResourceOwnerPasswordAsync(
                    ContactLogin,
                    ContactPassword));
        }

        /// <summary>
        /// Refreshes token using OAUTH2 client.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token.
        /// </param>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> RefreshTokenUsingOAuth2ClientAsync(string refreshToken)
        {
            // a special method to call the endpoint with a 'grant_type' = 'refresh_token'
            return await CallTokenUsingOAuth2ClientAsync(client =>
                client.RequestRefreshTokenAsync(refreshToken));
        }

        /// <summary>
        /// Revokes token using OAUTH2 client.
        /// </summary>
        /// <param name="refreshToken">
        /// The refresh token.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task RevokeTokenUsingOAuth2ClientAsync(string refreshToken)
        {
            // here we call the endpoint with a custom grant type
            var response = await CallUsingOAuth2ClientAsync(client =>
                client.RequestCustomAsync(new Dictionary<string, string>
                    {
                        { "grant_type", "revoke_token" },
                        { "refresh_token", refreshToken }
                    }));

            // actually, 'grant_type' = 'revoke_token' doesn't return a new token
            // and client will treat this case result as an error
            // so we work-around it
            if (!response.IsHttpError || response.HttpErrorStatusCode != HttpStatusCode.NoContent)
            {
                CheckOAuth2ClientError(response);
            }
        }

        /// <summary>
        /// Calls token using OAUTH2 client.
        /// </summary>
        /// <param name="callToken">
        /// The call token function.
        /// </param>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CallTokenUsingOAuth2ClientAsync(
            Func<OAuth2Client, Task<TokenResponse>> callToken)
        {
            var response = await CallUsingOAuth2ClientAsync(callToken);

            // it's a wrapper for 'password' and 'refresh_token' grant types
            // which return new token and we must use it
            CheckOAuth2ClientError(response);

            return new TokenModel
            {
                TokenType = response.TokenType,
                ExpiresIn = response.ExpiresIn,
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }

        /// <summary>
        /// Calls using OAUTH2 client.
        /// </summary>
        /// <param name="call">
        /// The call function.
        /// </param>
        /// <returns>
        /// The response.
        /// </returns>
        private static async Task<TokenResponse> CallUsingOAuth2ClientAsync(
            Func<OAuth2Client, Task<TokenResponse>> call)
        {
            // call an Authorization Server endpoint using Thinktecture.IdentityModel.Client.OAuth2Client
            var client = new OAuth2Client(
                new Uri(AuthorizationServerEndpointAddress),
                ClientID,
                ClientSecret)
            {
                Timeout = HttpTimeout
            };

            return await call(client);
        }

        /// <summary>
        /// Checks OAUTH2 client for an error.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        private static void CheckOAuth2ClientError(TokenResponse response)
        {
            // we check two types of errors in client
            if (response.IsError)
            {
                // the HTTP error has priority
                if (response.IsHttpError)
                {
                    // basically, HttpClient does the same thing in EnsureSuccessStatusCode method
                    throw new HttpRequestException(
                        string.Format(
                            "Response status code does not indicate success: {0} ({1})",
                            response.HttpErrorStatusCode,
                            response.HttpErrorReason));
                }

                // something went totally wrong
                throw new Exception(response.Error);
            }
        }

        #endregion

        #region Handle resources using HTTP client

        /// <summary>
        /// Demonstrates resource handling using HTTP client.
        /// </summary>
        /// <returns>
        /// The created account id.
        /// </returns>
        private static async Task<string> HandleResourcesUsingHttpClient()
        {
            // in this scenario we work with Resouse Server using HttpClient
            // consider official documentation https://cp.serverdata.net/webservices/RestAPI/docs-ui/index
            
            // the first thing we need is an active access token
            // we create a new one here, but you should reuse one created before (and refresh it from time to time)
            Write("3. Demonstrating resource handling using HTTP client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingOAuth2ClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");

            // we won't need token_type, expires_in and refresh_token for test purposes
            var accessToken = token.AccessToken;
            Write("Creating end-user account...");
            var account = await CreateEndUserAccount(accessToken);
            Write($"The end-user account was created:\r\n{Dump(account)}\r\n");

            var accountID = account.AccountID;
            Write("Accepting account MSA...");
            await AcceptAccountMsa(accessToken, accountID);
            Write("The account MSA was accepted.\r\n");

            Write("Changing account plan...");
            await ChangeAccountPlan(accessToken, accountID);
            Write("The account plan was changed.\r\n");

            Write("Creating an account contact...");
            var contact = await CreateAccountContact(accessToken, accountID);
            Write($"The account contact was created:\r\n{Dump(contact)}\r\n");

            Write("Creating an account limit...");
            var limit = await CreateAccountLimit(accessToken, accountID);
            Write($"The account limit was created:\r\n{Dump(limit)}\r\n\r\n");

            // just for demo purposes
            Write("Revoking a token...");
            await RevokeTokenUsingOAuth2ClientAsync(token.RefreshToken);
            Write("The token was revoked.\r\n\r\n");

            return accountID;
        }

        /// <summary>
        /// Creates end-user account.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>
        /// The end-user account.
        /// </returns>
        private static async Task<AccountGetModel> CreateEndUserAccount(string accessToken)
        {
            // generate a lot of dummy data to create a new end-user account
            var userName = $"imqa-usrapi{CreateRandomString(12)}";

            var address = new AddressModel
            {
                Country = "United States",
                State = "California",
                City = "SunnyVale",
                Street = "150 Mathilda Pl Ste 104",
                Zip = "94086-6010"
            };

            var accountToCreate = new AccountCreateModel
            {
                // type is an important item within this same
                // 'cause partner account should be handled in different way in some points
                Type = AccountTypeModel.EndUser,
                General = new AccountGeneralModel
                {
                    // credentials
                    UserName = userName,
                    Password = "qwe123!@#",

                    // you need to know the parent partner account id to create end-user account under it
                    ParentAccountID = "0158A13EF5D74E2D8CCD34C0E87F5034",

                    // account contact owner data
                    Name = userName,
                    Email = $"{userName}@qa.qa"
                },
                Company = new CompanyModel
                {
                    Name = userName,
                    Phone = "1234567890",
                    Address = address
                },
                Payment = new PaymentModel
                {
                    Name = userName,
                    Phone = "1234567890",
                    Address = address,
                    Type = PaymentTypeModel.CreditCard,
                    CreditCard = new PaymentCreditCardModel
                    {
                        Type = "VISA",
                        CardNumber = "4111111111111111",
                        ExpirationDate = "04/18",
                        SecurityCode = "111"
                    }
                },

                // plan name is required for end-user account type
                PlanName = "API_PL"
            };

            // consider https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_management/AccountsV1_PostAccount
            return await CallUsingHttpClientAsync<AccountCreateModel, AccountGetModel>(
                $"{ResourceServerEndpointAddress}/accounts",
                HttpMethod.Post,
                accessToken,
                accountToCreate);
        }

        /// <summary>
        /// Accept account MSA.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task AcceptAccountMsa(
            string accessToken, 
            string accountID)
        {
            // please not that MSA can be accepted only once
            var msaToAccept = new MsaModel { IsAccepted = true };

            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_Master_Service_Agreement/AccountMsaV1_PostMsa
            await CallUsingHttpClientAsync<MsaModel, object>(
                $"{ResourceServerEndpointAddress}/accounts/{accountID}/msa",
                HttpMethod.Post,
                accessToken,
                msaToAccept);
        }

        /// <summary>
        /// Changes account plan.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task ChangeAccountPlan(
            string accessToken, 
            string accountID)
        {
            // please note that your can't change the current plan to the same
            var planToChange = new PlanUpdateModel { Name = "IAM_PL" };

            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_billing_plans/AccountPlansV1_PostPlan
            await CallUsingHttpClientAsync<PlanUpdateModel, object>(
                $"{ResourceServerEndpointAddress}/accounts/{accountID}/plans",
                HttpMethod.Post,
                accessToken,
                planToChange);

            PlanGetModel currentPlan;
            do
            {
                // due to current endpoint method implementation limitation polling is required
                Write("Waiting for the account plan change completion...");
                Thread.Sleep(10000);

                // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_billing_plans/AccountPlansV1_GetPlans
                currentPlan = (await CallUsingHttpClientAsync<PageModel<PlanGetModel>>(
                    $"{ResourceServerEndpointAddress}/accounts/{accountID}/plans?isCurrent=true&skip=0&take=1",
                    HttpMethod.Get,
                    accessToken)).Items[0];

                // poll until current plan name was changed to the new one
            }
            while (currentPlan.Name != planToChange.Name);
        }

        /// <summary>
        /// Creates account contact.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The account contact.
        /// </returns>
        private static async Task<ContactGetModel> CreateAccountContact(
            string accessToken, 
            string accountID)
        {
            // generate dummy data to create a new end-user account contact
            var name = $"imqa-cntapi{CreateRandomString(16)}";
            var login = $"{name}@qa.qa";
            var contactToCreate = new ContactCreateModel
            {
                Login = login,
                Name = name,
                Email = login,
                AlternativeEmail = login,
                Phone = "1234567890",
                CellularPhone = "1234567890",
                Password = "qwe123!@#",
                AccessRoleNames = new[]
                {
                    "ContactManager",
                    "TechnicalAdministrator",
                    "SharePoint"
                }
            };

            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_contacts/AccountContactsV1_PostContact_0
            return await CallUsingHttpClientAsync<ContactCreateModel, ContactGetModel>(
                $"{ResourceServerEndpointAddress}/accounts/{accountID}/contacts",
                HttpMethod.Post,
                accessToken,
                contactToCreate);
        }

        /// <summary>
        /// Creates account limit.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The account limit.
        /// </returns>
        private static async Task<LimitModel> CreateAccountLimit(
            string accessToken, 
            string accountID)
        {
            // please note that limit name should be one of the defined on plan
            // also there is separate method to update already overriden limit
            var limitToCreate = new LimitModel
            {
                Name = "Iam_usersMax",
                Value = 10m
            };

            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_limits_(applicable_to_end-user_accounts%2C_to_enforce_prepaid_billing_model)/AccountLimitsV1_PostLimit
            return await CallUsingHttpClientAsync<LimitModel, LimitModel>(
                $"{ResourceServerEndpointAddress}/accounts/{accountID}/limits",
                HttpMethod.Post,
                accessToken,
                limitToCreate);
        }

        /// <summary>
        /// Invokes HTTP client.
        /// </summary>
        /// <param name="uri">
        /// The request URI.
        /// </param>
        /// <param name="method">
        /// The request HTTP method.
        /// </param>
        /// <param name="accessToken">
        /// The request access token.
        /// </param>
        /// <param name="content">
        /// The request object.
        /// </param>
        /// <typeparam name="TRequest">
        /// The request object type.
        /// </typeparam>
        /// <typeparam name="TResponse">
        /// The response object type.
        /// </typeparam>
        /// <returns>
        /// The response object.
        /// </returns>
        private static async Task<TResponse> CallUsingHttpClientAsync<TRequest, TResponse>(
            string uri,
            HttpMethod method,
            string accessToken,
            TRequest content)
            where TRequest : class
        {
            // wrapper over generic method where request content should be sent as JSON
            // we will use default conventions
            var requestJson = JsonConvert.SerializeObject(
                content,
                Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var requestContent = new StringContent(
                requestJson, 
                Encoding.UTF8, 
                "application/json");
            return await CallUsingHttpClientAsync<TResponse>(
                uri,
                method,
                accessToken,
                requestContent);
        }

        /// <summary>
        /// Invokes HTTP client.
        /// </summary>
        /// <param name="uri">
        /// The request URI.
        /// </param>
        /// <param name="method">
        /// The request HTTP method.
        /// </param>
        /// <param name="accessToken">
        /// The request access token.
        /// </param>
        /// <param name="content">
        /// The request content.
        /// </param>
        /// <typeparam name="TResponse">
        /// The response object type.
        /// </typeparam>
        /// <returns>
        /// The response object.
        /// </returns>
        private static async Task<TResponse> CallUsingHttpClientAsync<TResponse>(
            string uri,
            HttpMethod method,
            string accessToken,
            HttpContent content = null)
        {
            // another wrapper over generic method where response content should be read from JSON
            var response = await CallUsingHttpClientAsync(uri, method, accessToken, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }

        /// <summary>
        /// Calls using HTTP client.
        /// </summary>
        /// <param name="uri">
        /// The request URI.
        /// </param>
        /// <param name="method">
        /// The request HTTP method.
        /// </param>
        /// <param name="accessToken">
        /// The request access token.
        /// </param>
        /// <param name="content">
        /// The request content.
        /// </param>
        /// <returns>
        /// The response.
        /// </returns>
        private static async Task<HttpResponseMessage> CallUsingHttpClientAsync(
            string uri,
            HttpMethod method,
            string accessToken,
            HttpContent content = null)
        {
            // general method to call Resource Server endpoint using HttpClient
            using (var client = new HttpClient { Timeout = HttpTimeout })
            {
                var request = CreateHttpClientRequest(uri, method, accessToken, content);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // response content should be read only for related messages
                // i.e. probably, 200 OK will have some content in body
                if (response.Content != null)
                {
                    await response.Content.LoadIntoBufferAsync();
                }

                return response;
            }
        }

        /// <summary>
        /// Creates HTTP client request.
        /// </summary>
        /// <param name="uri">
        /// The request URI.
        /// </param>
        /// <param name="method">
        /// The request HTTP method.
        /// </param>
        /// <param name="accessToken">
        /// The request access token.
        /// </param>
        /// <param name="content">
        /// The request content.
        /// </param>
        /// <returns>
        /// The HTTP client request.
        /// </returns>
        private static HttpRequestMessage CreateHttpClientRequest(
            string uri,
            HttpMethod method,
            string accessToken,
            HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, uri);

            // setup Bearer token authorization
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (content != null)
            {
                request.Content = content;
            }

            return request;
        }

        #endregion

        #region Handle resources using API client

        /// <summary>
        /// Demonstrates resource handling using API client.
        /// </summary>
        /// <param name="accountID">
        /// The existent account id.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task HandleResourcesUsingApiClient(string accountID)
        {
            // in this scenario we work with Resouse Server using classes generated by Swagger Code Generator
            // consider https://github.com/swagger-api/swagger-codegen
            // and official documentation https://cp.serverdata.net/webservices/RestAPI/docs-ui/index

            // again, we need an access token to work with API
            Write("4. Demonstrating resource handling using API client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingOAuth2ClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");

            var accessToken = token.AccessToken;
            Write("Disabling an account...");
            await ChangeAccountState(accessToken, accountID, "disabled");
            Write("The account was disabled.\r\n");

            Write("Enabling an account...");
            await ChangeAccountState(accessToken, accountID, "enabled");
            Write("The account was enabled.\r\n");

            Write("Deleting an account...");
            await DeleteAccount(accessToken, accountID);
            Write("The account was deleted.\r\n");

            // just for demo purposes
            Write("Revoking a token...");
            await RevokeTokenUsingOAuth2ClientAsync(token.RefreshToken);
            Write("The token was revoked.\r\n\r\n");
        }

        /// <summary>
        /// Changes account state.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <param name="accountState">
        /// The account state.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task ChangeAccountState(
            string accessToken,
            string accountID,
            string accountState)
        {
            // please note that the current state can't be changed to the same
            var stateToChange = new StateV1Model(accountState);
            var accountStateClient = CreateApiAccessor<AccountsStateApi>(accessToken);

            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_state_(enabled%2Fdisabled)/AccountStateV1_PostState
            await accountStateClient.AccountStateV1PostStateAsync(
                accountID,
                stateToChange);

            StateV1Model currentState;
            do
            {
                // again, due to current endpoint method implementation limitation polling is required
                Console.WriteLine("Waiting for the account state change completion...");
                Thread.Sleep(10000);

                // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_state_(enabled%2Fdisabled)/AccountStateV1_GetState
                currentState = await accountStateClient
                    .AccountStateV1GetStateAsync(accountID);

                // codegen also implements models equality
            }
            while (!Equals(currentState, stateToChange));
        }

        /// <summary>
        /// Deletes account.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <param name="accountID">
        /// The account id.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        private static async Task DeleteAccount(
            string accessToken, 
            string accountID)
        {
            // account deletion is applicable only to end-user account type
            // consider http://localhost:6615/webservices/restapi/docs-ui/index#!/Account_management/AccountsV1_DeleteAccount
            await CreateApiAccessor<AccountsApi>(accessToken)
                .AccountsV1DeleteAccountAsync(
                    accountID, 
                    "Demo reason", 
                    "Demo comments");
        }

        /// <summary>
        /// Creates API accessor.
        /// </summary>
        /// <typeparam name="TApiAccessor">
        /// The API accessor type.
        /// </typeparam>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>
        /// The API accessor.
        /// </returns>
        private static TApiAccessor CreateApiAccessor<TApiAccessor>(string accessToken)
            where TApiAccessor : IApiAccessor, new()
        {
            // a generic method create api accessor generated by codegen
            const string AuthorizationHeaderKey = "Authorization";

            // version suffix should be ommited in path
            var configuration = new Configuration(new ApiClient(ResourceServerEndpointAddressBase));

            // add 'Bearer' token authorization
            configuration.AddApiKeyPrefix(AuthorizationHeaderKey, "Bearer");
            configuration.AddApiKey(AuthorizationHeaderKey, accessToken);

            return new TApiAccessor { Configuration = configuration };
        }

        #endregion

        #region Utility

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private static void Write(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Converts an object to a string.
        /// </summary>
        /// <param name="object">
        /// The source object.
        /// </param>
        /// <returns>
        /// The string representation of the object
        /// </returns>
        private static string Dump(object @object)
        {
            return JsonConvert.SerializeObject(
                @object, 
                Formatting.Indented);
        }

        /// <summary>
        /// Creates random string.
        /// </summary>
        /// <param name="length">
        /// The random string length.
        /// </param>
        /// <returns>
        /// The random string.
        /// </returns>
        private static string CreateRandomString(int length)
        {
            const string RandomCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var characters = new char[length];
            for (var i = 0; i < length; i++)
            {
                characters[i] = RandomCharacters[random.Next(RandomCharacters.Length)];
            }

            return new string(characters);
        }

        #endregion
    }
}
