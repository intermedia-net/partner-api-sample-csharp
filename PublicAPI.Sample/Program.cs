// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Intermedia">
//   Copyright © Intermedia.net, Inc. 1995 - 2016. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hosting.PublicAPI.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        /// This one is implemented with 'grant_type' = 'client_credentials' and has version 'v1'.
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
                // Entire async workflow is wrapped into separate handler.
                MainAsync().Wait();
                Write("Demo completed.");
            }
            catch (Exception ex)
            {
                // Usually, relevant exception should be extracted from AggregateException.
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
            // Four sequential demo steps.
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
            // All errors other than AggregateException should be handled in a standard way.
            if (!(source is AggregateException) || source.InnerException == null)
            {
                return source;
            }

            // The first meaningful thing should be here:
            var innerException = source.InnerException;

            // HTTP client timeout case - it can be work-arounded in a special way.
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
            // Create a new token for the client.
            Write("1. Demonstrating token handling using HTTP client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingHttpClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");
        }

        /// <summary>
        /// Creates token using HTTP client.
        /// </summary>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CreateTokenUsingHttpClientAsync()
        {
            // Consider https://tools.ietf.org/html/rfc6749#section-4.4
            var requestParameters = new Dictionary<string, string>
            {
                // Client credentials credentials flow.
                { "grant_type", "client_credentials" },

                // Authentication part, your client credentials.
                { "client_id", ClientID },
                { "client_secret", ClientSecret }
            };

            // It's a wrapper for 'client_credentials' grant type
            // which return new token within content and we must parse it.
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
            // Call an Authorization Server endpoint using the HttpClient.
            using (var client = new HttpClient { Timeout = HttpTimeout })
            {
                // All Authorization Server calls must be done using 'POST' HTTP verb.
                // Request content must be provided in 'application/x-www-form-urlencoded' MIME type.
                // Please note that you can authenticate (pass client_id and client_secret) using Basic Authentication.
                // Consider https://tools.ietf.org/html/rfc2617
                var response = await client.PostAsync(
                    AuthorizationServerEndpointAddress,
                    new FormUrlEncodedContent(requestParameters));

                response.EnsureSuccessStatusCode();
                await response.Content.LoadIntoBufferAsync();

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
            // The workflow is absolutely the same as in demo #1.
            // The only difference is that we use Thinktecture.IdentityModel Client.
            // Consider https://github.com/IdentityServer
            Write("2. Demonstrating token handling using OAUTH2 client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingOAuth2ClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");
        }

        /// <summary>
        /// Creates token using OAUTH2 client.
        /// </summary>
        /// <returns>
        /// The token.
        /// </returns>
        private static async Task<TokenModel> CreateTokenUsingOAuth2ClientAsync()
        {
            var response = await CallUsingOAuth2ClientAsync(client =>
                client.RequestClientCredentialsAsync());

            CheckOAuth2ClientError(response);

            return new TokenModel
            {
                TokenType = response.TokenType,
                ExpiresIn = response.ExpiresIn,
                AccessToken = response.AccessToken
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
            // Call an Authorization Server endpoint using Thinktecture.IdentityModel.Client.OAuth2Client.
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
            // We check two types of errors in client.
            if (response.IsError)
            {
                // The HTTP error has priority.
                if (response.IsHttpError)
                {
                    // Basically, HttpClient does the same thing in EnsureSuccessStatusCode method.
                    throw new HttpRequestException(
                        string.Format(
                            "Response status code does not indicate success: {0} ({1})",
                            response.HttpErrorStatusCode,
                            response.HttpErrorReason));
                }

                // Something went totally wrong.
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
            // In this scenario we work with Resouse Server using HttpClient.
            // See the official documentation https://cp.serverdata.net/webservices/RestAPI/docs-ui/index

            // The first thing we need is an active access token.
            Write("3. Demonstrating resource handling using HTTP client.\r\n");
            Write("Creating a token...");
            var token = await CreateTokenUsingOAuth2ClientAsync();
            Write($"The token was created:\r\n{Dump(token)}\r\n");

            // We don't need token_type and expires_in for this demo.
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
            // Generate a unique dummy name, to make sure there is no conflict with existing accounts.
            var userName = $"imqa-usrapi{CreateRandomString(12)}";

            // Use Intermedia's HQ address - for demonstartion purposes only.
            // Please use real enduser address, or your own legal address for real accounts.
            var address = new AddressModel
            {
                Country = "United States",
                State = "California",
                City = "Mountain View",
                Street = "825 E. Middlefield Rd",
                Zip = "94043-4025"
            };

            var accountToCreate = new AccountCreateModel
            {
                // Account type is always 'EndUser' for regular partners. 
                // You only need to specify 'Partner' when you create sub-partners in Distributor model.
                Type = AccountTypeModel.EndUser,
                General = new AccountGeneralModel
                {
                    // Credentials:
                    UserName = userName,
                    Password = $"{CreateRandomString(8)}_!@#",

                    // In Distributor model, you have to specify the sub-parent partner account id, to create end-user account within its container. 
                    // You do not need this in regular partner model - by default, end-user account are created in your container.
                    // ParentAccountID = "0158A13EF5D74E2D8CCD34C0E87F5034",

                    // Account contact owner data:
                    Name = $"Account Owner for {userName}",
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
                    Type = PaymentTypeModel.PaperCheck
                    /*
                    
                    You should use credit cards only if you process end-user payments through Intermedia-provided payment processor.
                    Please contact your Customer Service representative if you would like to set one up.

                    Type = PaymentTypeModel.CreditCard,
                    CreditCard = new PaymentCreditCardModel
                    {
                        Type = "VISA",
                        CardNumber = "4111111111111111",
                        ExpirationDate = "04/18",
                        SecurityCode = "111"
                    }
                    */
                },

                // Plan name is required for end-user account type. 
                // We use the most popular one here.
                PlanName = "E2016_Exch_1"
            };

            // Please take a look at online documentation:
            // https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_management/AccountsV1_PostAccount
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
            // Please note that Master Service Agreemnet (MSA) can be accepted only once.
            var msaToAccept = new MsaModel { IsAccepted = true };

            // Please take a look at online documentation: 
            // https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_Master_Service_Agreement/AccountMsaV1_PostMsa
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
            // Please note that your can't change the current plan to the same plan.
            var planToChange = new PlanUpdateModel { Name = "IAM_PL" };

            // Please take a look at online documentation:
            // https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_billing_plans/AccountPlansV1_PostPlan
            await CallUsingHttpClientAsync<PlanUpdateModel, object>(
                $"{ResourceServerEndpointAddress}/accounts/{accountID}/plans",
                HttpMethod.Post,
                accessToken,
                planToChange);

            PlanGetModel currentPlan;
            do
            {
                // Due to the API implementation specifics, polling is required.
                Write("Waiting for the account plan change completion...");
                Thread.Sleep(10000);

                // See https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_billing_plans/AccountPlansV1_GetPlans
                currentPlan = (await CallUsingHttpClientAsync<PageModel<PlanGetModel>>(
                    $"{ResourceServerEndpointAddress}/accounts/{accountID}/plans",
                    HttpMethod.Get,
                    accessToken)).Items.Single(item => item.IsCurrent.Value);

                // Poll until current plan name was changed to the new one.
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
            // Generate dummy data to create a new end-user account contact.
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
                Password = $"{CreateRandomString(8)}!@#",

                // For the full list of available roles, see https://FAQ.intermedia.net/Article/23265
                AccessRoleNames = new[]
                {
                    "ContactManager",
                    "TechnicalAdministrator",
                    "SharePoint"
                }
            };

            // Consider https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_contacts/AccountContactsV1_PostContact_0
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
            // Note that limit name should a valid limit defined in the billing plan.
            // Please use PUT method to update a previously configured limit.
            var limitToCreate = new LimitModel
            {
                Name = "Iam_usersMax",
                Value = 10m
            };

            // See https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_limits_(applicable_to_end-user_accounts%2C_to_enforce_prepaid_billing_model)/AccountLimitsV1_PostLimit
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
            // Wrapper over generic method where request content should be sent as JSON.
            // We are using default conventions.
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
            // Another wrapper over generic method where response content should be read from JSON.
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
            // General method to call Resource Server endpoint using HttpClient.
            using (var client = new HttpClient { Timeout = HttpTimeout })
            {
                var request = CreateHttpClientRequest(uri, method, accessToken, content);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Response content should be read only for related messages
                // i.e. probably, 200 OK will have some content in body.
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

            // Detup Bearer token authorization.
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
            // In this scenario we work with Resouse Server using classes generated by Swagger Code Generator.
            // Please refer to Swagger project at https://github.com/swagger-api/swagger-codegen
            // and Swagger-generated API documentation at https://cp.serverdata.net/webservices/RestAPI/docs-ui/index

            // We need an access token to work with API.
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
            // Please note that the state cannot be changed to the same state, i.e. enabled -> enabled is not allowed.
            var stateToChange = new StateV1Model(accountState);
            var accountStateClient = CreateApiAccessor<AccountsStateApi>(accessToken);

            // See https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_state_(enabled%2Fdisabled)/AccountStateV1_PostState
            await accountStateClient.AccountStateV1PostStateAsync(
                accountID,
                stateToChange);

            StateV1Model currentState;
            do
            {
                // Due to the API implementation specifics, polling is required.
                Console.WriteLine("Waiting for the account state change completion...");
                Thread.Sleep(10000);

                // See https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_state_(enabled%2Fdisabled)/AccountStateV1_GetState
                currentState = await accountStateClient
                    .AccountStateV1GetStateAsync(accountID);

                // Codegen also implements model equality through IEquatable interface.
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
            // Account deletion is applicable only to end-user account type:
            // https://cp.serverdata.net/webservices/restapi/docs-ui/index#!/Account_management/AccountsV1_DeleteAccount
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
            // A generic method create api accessor generated by codegen.
            const string AuthorizationHeaderKey = "Authorization";

            // Version suffix should be ommited in path.
            var configuration = new Configuration(new ApiClient(ResourceServerEndpointAddressBase));

            // Add 'Bearer' token authorization.
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