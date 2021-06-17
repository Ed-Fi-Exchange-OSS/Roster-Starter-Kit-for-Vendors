using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public class StaffService : ApiService
    {
        private readonly IDataService _rosterDataService;

        public StaffService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(responseHandleService, apiFacade)
        {
            _rosterDataService = rosterDataService;
        }

        public async Task<List<Staff>> ReadAllAsync()
        {
            var staff = await _rosterDataService.ReadAllAsync<RosterStaffComposite>();
            return staff.Select(st => JsonConvert.DeserializeObject<Staff>(st.Content)).ToList();
        }

        public async Task Save(List<Staff> staffResources)
        {
            var staffList = staffResources.Select(staff =>
                new RosterStaffComposite { Content = JsonConvert.SerializeObject(staff), ResourceId = staff.Id}).ToList();
            await _rosterDataService.SaveAsync(staffList);
        }

        public async Task<ExtendedInfoResponse<List<Staff>>> GetAllStaffWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync<StaffsApi, Staff>(
                ApiRoutes.StaffsComposite,
                async (api, offset, limit) =>
                    await api.GetStaffsWithHttpInfoAsync(offset, limit));
        }
    }
}
