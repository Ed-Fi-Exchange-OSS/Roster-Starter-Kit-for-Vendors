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
    public class StaffService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public StaffService(IDataService dataService
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

        public async Task<IEnumerable<EdFiStaff>> ReadAllAsync()
        {
            var staffResources = await _dataService.ReadAllAsync<RosterStaffResource>();
            return staffResources.Select(staff => JsonConvert.DeserializeObject<EdFiStaff>(staff.Content)).ToList();
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncStaff(long minVersion, long maxVersion)
        {
            var staffApi = await _apiFacade.GetApiClassInstance<StaffsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiStaff>>();
            var currResponseRecordCount = 0;

            if (minVersion >= maxVersion)
                return new DataSyncResponseModel
                {
                    ResourceName = $"No pending changes to sync for {ResourceTypes.Staff}"
                };
            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.StaffsResource, offset, limit);
                ApiResponse<List<EdFiStaff>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await staffApi.GetStaffsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        staffApi = await _apiFacade.GetApiClassInstance<StaffsApi>(true);
                        currentApiResponse = await staffApi.GetStaffsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            // Sync retrieved records to local db
            var staffResources = response.FullDataSet.Select(staff =>
                new RosterStaffResource { Content = JsonConvert.SerializeObject(staff), ResourceId = staff.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(staffResources);

            var deletesResponse =
                await _apiService.GetAllResources<StaffsApi, DeletedResource>(
                    $"{ApiRoutes.StaffsResource}/deletes",
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
