using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class SchoolService : ApiService<SchoolsApi>
    {
        private readonly IDataService _dataService;
        private readonly ChangeQueryService _changeQueryService;

        public SchoolService(
            IDataService dataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade,
            ChangeQueryService changeQueryService)
            : base(responseHandleService, apiFacade)
        {
            _dataService = dataService;
            _changeQueryService = changeQueryService;
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncResources(long minVersion, long maxVersion)
        {
            var queryParams = new Dictionary<string, string> { { "minChangeVersion", minVersion.ToString() },
                { "maxChangeVersion", maxVersion.ToString() } };

            var response =
                await GetAllResources(
                    $"{ApiRoutes.SchoolsResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetSchoolsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var schools = response.FullDataSet.Select(school =>
                new RosterSchoolResource { Content = JsonConvert.SerializeObject(school), ResourceId = school.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(schools);

            var deletesResponse =
                await GetAllResources(
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
