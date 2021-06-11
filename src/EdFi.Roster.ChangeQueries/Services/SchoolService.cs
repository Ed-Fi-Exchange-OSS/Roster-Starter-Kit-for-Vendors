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
using DeletedResource = EdFi.Roster.Sdk.Models.Resources.DeletedResource;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class SchoolService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public SchoolService(IDataService dataService
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

        public async Task<IEnumerable<EdFiSchool>> ReadAllAsync()
        {
            var schools = await _dataService.ReadAllAsync<RosterSchoolResource>();
            return schools.Select(school => JsonConvert.DeserializeObject<EdFiSchool>(school.Content)).ToList();
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncSchools(long minVersion, long maxVersion)
        {
            var schoolsApi = await _apiFacade.GetApiClassInstance<SchoolsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiSchool>>();
            var currResponseRecordCount = 0;

            if (minVersion >= maxVersion)
                return new DataSyncResponseModel
                {
                    ResourceName = $"No pending changes to sync for {ResourceTypes.Schools}"
                };
            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.SchoolsResource, offset, limit);
                ApiResponse<List<EdFiSchool>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await schoolsApi.GetSchoolsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        schoolsApi = await _apiFacade.GetApiClassInstance<SchoolsApi>(true);
                        currentApiResponse = await schoolsApi.GetSchoolsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            // Sync retrieved records to local db
            var schools = response.FullDataSet.Select(school =>
                new RosterSchoolResource { Content = JsonConvert.SerializeObject(school), ResourceId = school.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(schools);

            var deletesResponse =
                await _apiService.GetAllResources<SchoolsApi, DeletedResource>(
                    $"{ApiRoutes.SchoolsResource}/deletes",
                    async (api, offset, limit) =>
                        await api.DeletesSchoolsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync deleted records to local db
            var deletedSchoolsCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedSchools = deletesResponse.FullDataSet.Select(school => school.Id).ToList();
                await _dataService.DeleteAllAsync<RosterSchoolResource>(deletedSchools);
                deletedSchoolsCount = deletedSchools.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.Schools);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Schools,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = deletedSchoolsCount
            };
        }
    }
}
