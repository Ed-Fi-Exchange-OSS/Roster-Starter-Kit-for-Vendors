using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;

namespace EdFi.Roster.Explorer.Services
{
    public class StaffService :
        ApiService<StaffsApi, Staff, RosterStaffComposite>
    {
        public StaffService(IDataService rosterDataService, IResponseHandleService responseHandleService, IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        protected override string ApiRoute => ApiRoutes.StaffsComposite;

        protected override async Task<ApiResponse<List<Staff>>> GetPageAsync(StaffsApi api, int offset, int limit)
            => await api.GetStaffsWithHttpInfoAsync(offset, limit);

        protected override string GetResourceId(Staff resource) => resource.Id;
    }
}
