using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;

namespace EdFi.Roster.ChangeQueries.Services
{
    public abstract class ApiService<TApiAccessor, TResource, TRecord>
        where TApiAccessor : IApiAccessor
        where TResource : class
        where TRecord : RosterDataRecord, new()
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        protected delegate Task<ApiResponse<List<T>>> GetPageAsync<T>(TApiAccessor api, int offset, int limit, int minChangeVersion, int maxChangeVersion);

        protected ApiService(IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        protected abstract string ApiRoute { get; }
        protected abstract string ResourceType { get; }

        protected abstract Task<ApiResponse<List<TResource>>> GetChangesAsync(TApiAccessor api, int offset, int limit, int minChangeVersion, int maxChangeVersion);
        protected abstract Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(TApiAccessor api, int offset, int limit, int minChangeVersion, int maxChangeVersion);

        protected async Task<ExtendedInfoResponse<List<T>>> GetAllResources<T>(
            string apiRoute, Dictionary<string,string> queryParams,
            int minChangeVersion, int maxChangeVersion,
            GetPageAsync<T> getPageAsync)
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
                     currentApiResponse = await getPageAsync(api, offset, limit, minChangeVersion, maxChangeVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int) HttpStatusCode.Unauthorized))
                    {
                        api = await _apiFacade.GetApiClassInstance<TApiAccessor>(true);
                        currentApiResponse = await getPageAsync(api, offset, limit, minChangeVersion, maxChangeVersion);
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
