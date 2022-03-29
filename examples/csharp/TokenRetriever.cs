using System.Net;
using System.Security.Authentication;
using RestSharp;

namespace get_students_by_grade
{
    public class TokenRetriever
    {
        private readonly string oauthUrl;
        private readonly string clientKey;
        private readonly string clientSecret;

        public TokenRetriever(string oauthUrl, string clientKey, string clientSecret)
        {
            this.oauthUrl = oauthUrl;
            this.clientKey = clientKey;
            this.clientSecret = clientSecret;
        }

        public string ObtainNewBearerToken()
        {
            var oauthClient = new RestClient(oauthUrl);
            return GetBearerToken(oauthClient);
        }


        private string GetBearerToken(IRestClient oauthClient)
        {
            var bearerTokenRequest = new RestRequest("oauth/token", Method.POST);
            bearerTokenRequest.AddParameter("Client_id", clientKey);
            bearerTokenRequest.AddParameter("Client_secret", clientSecret);
            bearerTokenRequest.AddParameter("Grant_type", "client_credentials");

            var bearerTokenResponse = oauthClient.Execute<BearerTokenResponse>(bearerTokenRequest);
            if (bearerTokenResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new AuthenticationException("Unable to retrieve an access token. Error message: " +
                                                  bearerTokenResponse.ErrorMessage);
            }

            if (bearerTokenResponse.Data.Error != null || bearerTokenResponse.Data.TokenType != "bearer")
            {
                throw new AuthenticationException(
                    "Unable to retrieve an access token. Please verify that your application secret is correct.");
            }

            return bearerTokenResponse.Data.AccessToken;
        }
    }

    internal class BearerTokenResponse
    {
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
        public string TokenType { get; set; }
        public string Error { get; set; }
    }
}
