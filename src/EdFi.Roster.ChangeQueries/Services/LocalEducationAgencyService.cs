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
    public class LocalEducationAgencyService
    {
        private readonly IDataService _dataService;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public LocalEducationAgencyService(IDataService dataService
            , ApiService apiService
            , ChangeQueryService changeQueryService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _changeQueryService = changeQueryService;
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncLocalEducationAgencies(long minVersion, long maxVersion)
        {
            var queryParams = new Dictionary<string, string> { { "minChangeVersion", minVersion.ToString() },
                { "maxChangeVersion", maxVersion.ToString() } };

            var response =
                await _apiService.GetAllResources<LocalEducationAgenciesApi, EdFiLocalEducationAgency>(
                    $"{ApiRoutes.LocalEducationAgenciesResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetLocalEducationAgenciesWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var leas = response.FullDataSet.Select(lea =>
                new RosterLocalEducationAgencyResource
                    {Content = JsonConvert.SerializeObject(lea), ResourceId = lea.Id}).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(leas);

            var deletesResponse =
                await _apiService.GetAllResources<LocalEducationAgenciesApi, DeletedResource>(
                    $"{ApiRoutes.LocalEducationAgenciesResource}/deletes", queryParams,
                    async (api, offset, limit) =>
                        await api.DeletesLocalEducationAgenciesWithHttpInfoAsync(
                            offset, limit, (int?) minVersion, (int?) maxVersion));

            // Sync deleted records to local db
            var deletedLeasCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedLeas = deletesResponse.FullDataSet.Select(lea => lea.Id).ToList();
                await _dataService.DeleteAllAsync<RosterLocalEducationAgencyResource>(deletedLeas);
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
