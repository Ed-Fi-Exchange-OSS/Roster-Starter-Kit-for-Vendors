using System;
using System.Net;
using System.Threading.Tasks;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;
using RestSharp;

namespace EdFi.Common
{
    public class BearerTokenService
    {
      
        private readonly ApiLogService _apiLogService;

        public BearerTokenService(ApiLogService apiLogService)
        {
            _apiLogService = apiLogService;
        }

        private string AccessToken { get; set; }

        public async Task<ApiResponse<BearerTokenResponse>> GetNewBearerTokenResponse(ApiSettings apiSettings)
        {
            var oauthClient = new RestClient(apiSettings.RootUrl);
            var bearerTokenRequest = new RestRequest("oauth/token", Method.POST);
            bearerTokenRequest.AddParameter("Client_id", apiSettings.Key);
            bearerTokenRequest.AddParameter("Client_secret", apiSettings.Secret);
            bearerTokenRequest.AddParameter("Grant_type", "client_credentials");

            var bearerTokenResponse = oauthClient.Execute<BearerTokenResponse>(bearerTokenRequest);

            await LogDetails(bearerTokenRequest, bearerTokenResponse);

            var headersMap = new Multimap<string, string>();

            foreach (var header in bearerTokenResponse.Headers)
            {
                headersMap.Add(header.Name, header.Value.ToString());
            }

            //return results
            return new ApiResponse<BearerTokenResponse>(bearerTokenResponse.StatusCode,
                headersMap, bearerTokenResponse.Data);
        }

        private async Task LogDetails(IRestRequest bearerTokenRequest, IRestResponse<BearerTokenResponse> bearerTokenResponse)
        {
            var apiLogEntry = new ApiLogEntry
            {
                LogDateTime = DateTime.Now,
                Method = bearerTokenRequest.Method.ToString(),
                StatusCode = bearerTokenResponse.StatusCode.ToString(),
                Content = string.IsNullOrEmpty(bearerTokenResponse.Data.Error)
                    ? "Access token retrieved successfully"
                    : bearerTokenResponse.Data.Error,
                Uri = bearerTokenResponse.ResponseUri.ToString()
            };
            await _apiLogService.WriteLog(apiLogEntry);
        }

        public async Task<string> GetBearerToken(ApiSettings apiSettings, bool refreshToken = false)
        {
            if (AccessToken != null && !refreshToken) return AccessToken;
            var response = await GetNewBearerTokenResponse(apiSettings);

            if (!string.IsNullOrEmpty(response.Data.Error) || response.StatusCode != HttpStatusCode.OK)
                throw new ApiException((int)response.StatusCode, response.Data.Error);

            AccessToken = response.Data.AccessToken;
            return AccessToken;
        }
    }
}
