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
    public class SectionService : ApiService
    {
        private readonly IDataService _dataService;
        private readonly ChangeQueryService _changeQueryService;

        public SectionService(
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
                await GetAllResources<SectionsApi, EdFiSection>(
                    $"{ApiRoutes.SectionsResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetSectionsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var sections = response.FullDataSet.Select(section =>
                new RosterSectionResource { Content = JsonConvert.SerializeObject(section), ResourceId = section.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(sections);

            var deletesResponse =
                await GetAllResources<SectionsApi, DeletedResource>(
                    $"{ApiRoutes.SectionsResource}/deletes", queryParams,
                    async (api, offset, limit) =>
                        await api.DeletesSectionsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync deleted records to local db
            var deletedSectionsCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedSections = deletesResponse.FullDataSet.Select(section => section.Id).ToList();
                await _dataService.DeleteAllAsync<RosterSectionResource>(deletedSections);
                deletedSectionsCount = deletedSections.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.Sections);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Sections,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = deletedSectionsCount
            };
        }
    }
}
