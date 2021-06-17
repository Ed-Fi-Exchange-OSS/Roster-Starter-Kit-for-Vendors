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
    public class LocalEducationAgencyService : ApiService<LocalEducationAgenciesApi>
    {
        private readonly IDataService _dataService;
        private readonly ChangeQueryService _changeQueryService;

        public LocalEducationAgencyService(
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
                    $"{ApiRoutes.LocalEducationAgenciesResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetLocalEducationAgenciesWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var records = response.FullDataSet.Select(x =>
                new RosterLocalEducationAgencyResource
                {
                    Content = JsonConvert.SerializeObject(x),
                    ResourceId = x.Id
                }).ToList();
            var countAdded = await _dataService.AddOrUpdateAllAsync(records);

            var deletesResponse =
                await GetAllResources(
                    $"{ApiRoutes.LocalEducationAgenciesResource}/deletes", queryParams,
                    async (api, offset, limit) =>
                        await api.DeletesLocalEducationAgenciesWithHttpInfoAsync(
                            offset, limit, (int?) minVersion, (int?) maxVersion));

            // Sync deleted records to local db
            var countDeleted = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var resourceIds = deletesResponse.FullDataSet.Select(x => x.Id).ToList();
                await _dataService.DeleteAllAsync<RosterLocalEducationAgencyResource>(resourceIds);
                countDeleted = resourceIds.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.LocalEducationAgencies);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.LocalEducationAgencies,
                AddedRecordsCount = countAdded,
                UpdatedRecordsCount = response.FullDataSet.Count - countAdded,
                DeletedRecordsCount = countDeleted
            };
        }
    }
}
