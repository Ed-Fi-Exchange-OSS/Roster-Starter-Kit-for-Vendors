using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class LocalEducationAgencyService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public LocalEducationAgencyService(IDataService dataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade
            , ApiService apiService
            , ChangeQueryService changeQueryService)
        {
            _dataService = dataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
            _apiService = apiService;
            _changeQueryService = changeQueryService;
        }

        public async Task<IEnumerable<EdFiLocalEducationAgency>> ReadAllAsync()
        {
            var leas = await _dataService.ReadAllAsync<RosterLocalEducationAgencyResource>();
            return leas.Select(lea => JsonConvert.DeserializeObject<EdFiLocalEducationAgency>(lea.Content)).ToList();
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncLocalEducationAgencies(long minVersion, long maxVersion)
        {
            var leaApi = await _apiFacade.GetApiClassInstance<LocalEducationAgenciesApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiLocalEducationAgency>>();
            var currResponseRecordCount = 0;

            if (minVersion >= maxVersion)
                return new DataSyncResponseModel
                {
                    ResourceName = $"No pending changes to sync for {ResourceTypes.LocalEducationAgencies}"
                };
            do
            {
                var errorMessage = string.Empty;
                var responseUri =
                    _apiFacade.BuildResponseUri(ApiRoutes.LocalEducationAgenciesResource, offset, limit);
                ApiResponse<List<EdFiLocalEducationAgency>> currentApiResponse = null;
                try
                {
                    currentApiResponse =
                        await leaApi.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit, (int?) minVersion,
                            (int?) maxVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int) HttpStatusCode.Unauthorized))
                    {
                        leaApi = await _apiFacade.GetApiClassInstance<LocalEducationAgenciesApi>(true);
                        currentApiResponse =
                            await leaApi.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit,
                                (int?) minVersion, (int?) maxVersion);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri,
                    errorMessage);

            } while (currResponseRecordCount >= limit);

            // Sync retrieved records to local db
            var leas = response.FullDataSet.Select(lea =>
                new RosterLocalEducationAgencyResource
                    {Content = JsonConvert.SerializeObject(lea), ResourceId = lea.Id}).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(leas);

            const string methodName = "DeletesLocalEducationAgenciesWithHttpInfoAsync";
            var deletesResponse =
                await _apiService.DeletedResources<LocalEducationAgenciesApi>(methodName, minVersion, maxVersion);
            // Sync deleted records to local db
            var deletedLeasCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedLeas = deletesResponse.FullDataSet.Select(lea =>
                    new RosterLocalEducationAgencyResource {ResourceId = lea.Id}).ToList();
                await _dataService.DeleteAllAsync(deletedLeas);
                deletedLeasCount = deletedLeas.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.LocalEducationAgencies);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.LocalEducationAgencies,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = deletedLeasCount
            };
        }
    }
}
