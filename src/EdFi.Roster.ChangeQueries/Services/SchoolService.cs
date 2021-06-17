using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Models.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class SchoolService
    {
        private readonly IDataService _dataService;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public SchoolService(IDataService dataService
            , ApiService apiService
            , ChangeQueryService changeQueryService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _changeQueryService = changeQueryService;
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncResources(long minVersion, long maxVersion)
        {
            var queryParams = new Dictionary<string, string> { { "minChangeVersion", minVersion.ToString() },
                { "maxChangeVersion", maxVersion.ToString() } };

            var response =
                await _apiService.GetAllResources<SchoolsApi, EdFiSchool>(
                    $"{ApiRoutes.SchoolsResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetSchoolsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var schools = response.FullDataSet.Select(school =>
                new RosterSchoolResource { Content = JsonConvert.SerializeObject(school), ResourceId = school.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(schools);

            var deletesResponse =
                await _apiService.GetAllResources<SchoolsApi, DeletedResource>(
                    $"{ApiRoutes.SchoolsResource}/deletes", queryParams,
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
