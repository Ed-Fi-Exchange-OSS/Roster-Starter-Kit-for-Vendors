using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Client;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public abstract class ApiService<TApiAccessor, TResource, TRecord>
        where TApiAccessor : IApiAccessor
        where TResource : class
        where TRecord : RosterDataRecord, new()
    {
        private readonly IDataService _rosterDataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        protected ApiService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
        {
            _rosterDataService = rosterDataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        protected delegate Task<ApiResponse<List<TResource>>> GetPageAsync(TApiAccessor api, int offset, int limit);

        protected async Task<ExtendedInfoResponse<List<TResource>>> GetAllResourcesWithExtendedInfoAsync(
            string apiRoute, GetPageAsync getPageAsync)
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

        public async Task<List<TResource>> ReadAllAsync()
        {
            var records = await _rosterDataService.ReadAllAsync<TRecord>();
            return records.Select(x => JsonConvert.DeserializeObject<TResource>(x.Content)).ToList();
        }

        public async Task Save(List<TResource> resources)
        {
            var records = resources.Select(x =>
                new TRecord
                {
                    Content = JsonConvert.SerializeObject(x),
                    ResourceId = GetResourceId(x)
                }).ToList();

            await _rosterDataService.SaveAsync(records);
        }

        protected abstract string GetResourceId(TResource resource);
    }
}
