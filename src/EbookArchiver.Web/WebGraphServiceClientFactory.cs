using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace EbookArchiver.OneDrive
{
    public class WebGraphServiceClientFactory : IGraphServiceClientFactory
    {
        protected readonly ITokenAcquisition _tokenAcquisition;

        public WebGraphServiceClientFactory(
            ITokenAcquisition tokenAcquisition) => _tokenAcquisition = tokenAcquisition;

        // Gets a Graph client configured with
        // the specified scopes
        public GraphServiceClient GetGraphClientForScopes(params string[] scopes) =>
            GetAuthenticatedGraphClient(async () =>
                {
                    string? token = await _tokenAcquisition
                        .GetAccessTokenForUserAsync(scopes);

                    // Uncomment to print access token to debug console
                    // This will happen for every Graph call, so leave this
                    // out unless you're debugging your token
                    //_logger.LogInformation($"Access token: {token}");

                    return token;
                }
            );

        /// <summary>
        /// If the Graph client is unable to get a token for the
        /// requested scopes, it throws this type of exception.
        /// </summary>
        /// <remarks>
        /// The sample apps call this within a try/catch cause. This will rethrow if it's an auth failure, and
        /// return otherwise, allowing the page to display an error message.
        ///
        /// If errors should just go to the global error handler, this does not need to be called.
        /// <see cref="AuthorizeForScopesAttribute"/> alone is all that's needed.
        /// </remarks>
        public static void InvokeAuthIfNeeded(ServiceException serviceException)
        {
            // Check if this failed because interactive auth is needed
            if (serviceException.InnerException is MicrosoftIdentityWebChallengeUserException)
            {
                // Throwing the exception causes Microsoft.Identity.Web to
                // take over, handling auth (based on scopes defined in the
                // AuthorizeForScopes attribute)
                throw serviceException;
            }
        }

        // Uses a page iterator to get all objects in a collection
        public static async Task<List<T>> GetAllPages<T>(
            GraphServiceClient graphClient,
            ICollectionPage<T> page)
        {
            var allItems = new List<T>();

            var pageIterator = PageIterator<T>.CreatePageIterator(
                graphClient, page,
                (item) =>
                {
                    // This code executes for each item in the
                    // collection
                    allItems.Add(item);
                    return true;
                }
            );

            await pageIterator.IterateAsync();

            return allItems;
        }

        // Uses a page iterator to get all directoryObjects
        // in a collection and cast them to a specific type
        // Will exclude any objects that cannot be case to the
        // requested type
        public static async Task<List<T>> GetAllPagesAsType<T>(
            GraphServiceClient graphClient,
            ICollectionPage<DirectoryObject> page) where T : class
        {
            var allItems = new List<T>();

            var pageIterator = PageIterator<DirectoryObject>.CreatePageIterator(
                graphClient, page,
                (item) =>
                {
                    // This code executes for each item in the
                    // collection
                    if (item is T t)
                    {
                        // Only add if the item is the requested type
                        allItems.Add(t);
                    }

                    return true;
                }
            );

            await pageIterator.IterateAsync();

            return allItems;
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(
            Func<Task<string>> acquireAccessToken) => new(
                new CustomAuthenticationProvider(acquireAccessToken)
            );

        private class CustomAuthenticationProvider : IAuthenticationProvider
        {
            private readonly Func<Task<string>> _acquireAccessToken;
            public CustomAuthenticationProvider(Func<Task<string>> acquireAccessToken) => _acquireAccessToken = acquireAccessToken;

            public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
            {
                string? accessToken = await _acquireAccessToken.Invoke();

                // Add the token in the Authorization header
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer", accessToken
                );
            }
        }
    }
}
