using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;

namespace EdFi.Roster.Services
{
    public class StaffService :
        ApiService<StaffsApi, Staff, RosterStaffComposite>
    {
        public StaffService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        public async Task<ExtendedInfoResponse<List<Staff>>> GetAllStaffWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync(
                ApiRoutes.StaffsComposite,
                async (api, offset, limit) =>
                    await api.GetStaffsWithHttpInfoAsync(offset, limit));
        }

        protected override string GetResourceId(Staff resource) => resource.Id;
    }
}
