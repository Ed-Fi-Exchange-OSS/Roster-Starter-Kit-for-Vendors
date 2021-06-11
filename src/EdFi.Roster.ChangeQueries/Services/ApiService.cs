using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class ApiService
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public delegate Task<ApiResponse<List<TResource>>> GetPageAsync<in TApiAccessor, TResource>(TApiAccessor api, int offset, int limit)
            where TApiAccessor : IApiAccessor;

        public ApiService(IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task<ExtendedInfoResponse<List<TResource>>> GetAllResources<TApiAccessor, TResource>(
            string apiRoute, GetPageAsync<TApiAccessor, TResource> getPageAsync)
            where TApiAccessor : IApiAccessor
            where TResource : class
        {
            var api = await _apiFacade.GetApiClassInstance<TApiAccessor>();
            var limit = 100;
            var offset = 0;
            var currResponseRecordCount = 0;
            var response = new ExtendedInfoResponse<List<TResource>>();
            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(apiRoute, offset, limit);
                ApiResponse<List<TResource>> currentApiResponse = null;
                try
                {
                     currentApiResponse = await getPageAsync(api, offset, limit);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int) HttpStatusCode.Unauthorized))
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

            return response;
        }
    }
}