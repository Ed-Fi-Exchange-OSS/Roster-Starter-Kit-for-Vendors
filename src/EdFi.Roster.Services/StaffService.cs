using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Services.ApiSdk;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public class StaffService
    {
        private readonly IRosterDataService _rosterDataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public StaffService(IRosterDataService rosterDataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _rosterDataService = rosterDataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task<IEnumerable<Staff>> ReadAllAsync()
        {
            var staff = await _rosterDataService.ReadAllAsync<RosterStaff>();
            return staff.Select(st => JsonConvert.DeserializeObject<Staff>(st.Content)).ToList();
        }

        public async Task Save(List<Staff> staffResources)
        {
            var staffList = staffResources.Select(staff =>
                new RosterStaff { Content = JsonConvert.SerializeObject(staff), ResourceId = staff.Id}).ToList();
            await _rosterDataService.SaveAsync(staffList);
        }

        public async Task<ExtendedInfoResponse<List<Staff>>> GetAllStaffWithExtendedInfoAsync()
        {
            var api = await _apiFacade.GetApiClassInstance<StaffsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<Staff>>();
            int currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.Staffs, offset, limit);
                ApiResponse<List<Staff>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await api.GetStaffsWithHttpInfoAsync(offset, limit);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        api = await _apiFacade.GetApiClassInstance<StaffsApi>(true); 
                        currentApiResponse = await api.GetStaffsWithHttpInfoAsync(offset, limit);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            response.GeneralInfo.TotalRecords = response.FullDataSet.Count;
            response.GeneralInfo.ResponseData = JsonConvert.SerializeObject(response.FullDataSet, Formatting.Indented);
            return response;
        }
    }
}
