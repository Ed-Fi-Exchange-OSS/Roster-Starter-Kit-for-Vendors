using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.ChangeQueries.Services
{
    public abstract class ApiService<TApiAccessor, TResource, TRecord>
        where TApiAccessor : IApiAccessor
        where TResource : class
        where TRecord : RosterDataRecord, new()
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public delegate Task<ApiResponse<List<T>>> GetPageAsync<T>(TApiAccessor api, int offset, int limit);

        protected ApiService(IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        protected abstract string ApiRoute { get; }
        protected abstract string ResourceType { get; }

        protected async Task<ExtendedInfoResponse<List<T>>> GetAllResources<T>(
            string apiRoute, Dictionary<string,string> queryParams, GetPageAsync<T> getPageAsync)
            where T : class
        {
            var api = await _apiFacade.GetApiClassInstance<TApiAccessor>();
            var limit = 100;
            var offset = 0;
            var currResponseRecordCount = 0;
            var response = new ExtendedInfoResponse<List<T>>();

            queryParams ??= new Dictionary<string, string>();
            do
            {
                var errorMessage = string.Empty;

                if (!queryParams.ContainsKey("offset"))
                    queryParams.Add("offset", offset.ToString());
                else
                    queryParams["offset"] = offset.ToString();

                if(!queryParams.ContainsKey("limit"))
                    queryParams.Add("limit", limit.ToString());
                else
                    queryParams["limit"] = limit.ToString();

                var responseUri = _apiFacade.BuildResponseUri(apiRoute, queryParams);

                ApiResponse<List<T>> currentApiResponse = null;
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

        protected abstract string GetResourceId(TResource resource);
    }
}
