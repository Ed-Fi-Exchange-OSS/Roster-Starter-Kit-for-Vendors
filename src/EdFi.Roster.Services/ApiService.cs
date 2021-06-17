using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public abstract class ApiService
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        protected ApiService(
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        protected delegate Task<ApiResponse<List<TResource>>> GetPageAsync<in TApiAccessor, TResource>(TApiAccessor api, int offset, int limit)
            where TApiAccessor : IApiAccessor;

        protected async Task<ExtendedInfoResponse<List<TResource>>> GetAllResourcesWithExtendedInfoAsync<TApiAccessor, TResource>(
            string apiRoute, GetPageAsync<TApiAccessor, TResource> getPageAsync)
            where TApiAccessor : IApiAccessor
            where TResource : class
        {
            var api = await _apiFacade.GetApiClassInstance<TApiAccessor>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<TResource>>();
            int currResponseRecordCount = 0;
            var queryParams = new Dictionary<string, string> { { "offset", offset.ToString() }, { "limit", limit.ToString() } };
            do
            {
                var errorMessage = string.Empty;
                queryParams["offset"] = offset.ToString();
                queryParams["limit"] = limit.ToString();
                var responseUri = _apiFacade.BuildResponseUri(apiRoute, queryParams);
                ApiResponse<List<TResource>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await getPageAsync(api, offset, limit);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        api = await _apiFacade.GetApiClassInstance<TApiAccessor>(true);
                        currentApiResponse = await getPageAsync(api, offset, limit);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);
            } while (currResponseRecordCount >= limit);

            response.GeneralInfo.TotalRecords = response.FullDataSet.Count;
            response.GeneralInfo.ResponseData = JsonConvert.SerializeObject(response.FullDataSet, Formatting.Indented);
            return response;
        }
    }
}
