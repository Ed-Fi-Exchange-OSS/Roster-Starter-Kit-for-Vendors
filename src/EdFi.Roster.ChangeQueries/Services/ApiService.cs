using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.ChangeQueries.Services
{
    public abstract class ApiService<TApiAccessor>
        where TApiAccessor : IApiAccessor
    {
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public delegate Task<ApiResponse<List<TResource>>> GetPageAsync<TResource>(TApiAccessor api, int offset, int limit);

        protected ApiService(IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        protected abstract string ApiRoute { get; }
        protected abstract string ResourceType { get; }

        protected async Task<ExtendedInfoResponse<List<TResource>>> GetAllResources<TResource>(
            string apiRoute, Dictionary<string,string> queryParams, GetPageAsync<TResource> getPageAsync)
            where TResource : class
        {
            var api = await _apiFacade.GetApiClassInstance<TApiAccessor>();
            var limit = 100;
            var offset = 0;
            var currResponseRecordCount = 0;
            var response = new ExtendedInfoResponse<List<TResource>>();

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
