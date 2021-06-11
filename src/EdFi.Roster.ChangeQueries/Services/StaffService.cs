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
    public class StaffService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public StaffService(IDataService dataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _dataService = dataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
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

            // TODO: Call Deletes endpoint to get deleted records and update local db accordingly

            // TODO: Update Change query table to reflect latest available version for Staff

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Staff,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = 0
            };
        }
    }
}
