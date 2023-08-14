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
            bearerTokenRequest.AddParameter("client_id", apiSettings.Key);
            bearerTokenRequest.AddParameter("client_secret", apiSettings.Secret);
            bearerTokenRequest.AddParameter("grant_type", "client_credentials");

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
            var data = bearerTokenResponse.Data;

            var apiLogEntry = new ApiLogEntry
            {
                LogDateTime = DateTime.Now,
                Method = bearerTokenRequest.Method.ToString(),
                StatusCode = bearerTokenResponse.StatusCode.ToString(),
                Content = $"{(int) bearerTokenResponse.StatusCode} ({bearerTokenResponse.StatusCode}) {data?.Error}",
                Uri = bearerTokenResponse.ResponseUri.ToString()
            };
            await _apiLogService.WriteLog(apiLogEntry);
        }

        public async Task<string> GetBearerToken(ApiSettings apiSettings, bool refreshToken = false)
        {
            if (AccessToken != null && !refreshToken) return AccessToken;
            var response = await GetNewBearerTokenResponse(apiSettings);

            var data = response.Data;

            if (data == null || !string.IsNullOrEmpty(data?.Error) || response.StatusCode != HttpStatusCode.OK)
                throw new ApiException((int)response.StatusCode, data?.Error);

            AccessToken = data.AccessToken;
            return AccessToken;
        }
    }
}
