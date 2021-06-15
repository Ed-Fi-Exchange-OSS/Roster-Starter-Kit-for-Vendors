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
using DeletedResource = EdFi.Roster.Sdk.Models.Resources.DeletedResource;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class StaffService
    {
        private readonly IDataService _dataService;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public StaffService(IDataService dataService
            , ApiService apiService
            , ChangeQueryService changeQueryService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _changeQueryService = changeQueryService;
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncStaff(long minVersion, long maxVersion)
        {
            var queryParams = new Dictionary<string, string> { { "minChangeVersion", minVersion.ToString() },
                { "maxChangeVersion", maxVersion.ToString() } };

            var response =
                await _apiService.GetAllResources<StaffsApi, EdFiStaff>(
                    $"{ApiRoutes.StaffsResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetStaffsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var staffResources = response.FullDataSet.Select(staff =>
                new RosterStaffResource { Content = JsonConvert.SerializeObject(staff), ResourceId = staff.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(staffResources);

            var deletesResponse =
                await _apiService.GetAllResources<StaffsApi, DeletedResource>(
                    $"{ApiRoutes.StaffsResource}/deletes", queryParams,
                    async (api, offset, limit) =>
                        await api.DeletesStaffsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync deleted records to local db
            var deletedStaffCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedStaffResources = deletesResponse.FullDataSet.Select(staff => staff.Id).ToList();
                await _dataService.DeleteAllAsync<RosterStaffResource>(deletedStaffResources);
                deletedStaffCount = deletedStaffResources.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.Staff);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Staff,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = deletedStaffCount
            };
        }
    }
}
