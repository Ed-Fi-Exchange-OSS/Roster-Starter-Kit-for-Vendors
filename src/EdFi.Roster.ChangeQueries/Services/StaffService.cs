using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class StaffService : ApiService<StaffsApi, EdFiStaff, RosterStaffResource>
    {
        public StaffService(
            IDataService dataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade,
            ChangeQueryService changeQueryService)
            : base(dataService, responseHandleService, apiFacade, changeQueryService)
        {
        }

        protected override string ApiRoute => ApiRoutes.StaffsResource;

        protected override string ResourceType => ResourceTypes.Staff;

        protected override async Task<ApiResponse<List<EdFiStaff>>> GetChangesAsync(StaffsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.GetStaffsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override async Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(StaffsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.DeletesStaffsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override string GetResourceId(EdFiStaff resource) => resource.Id;
    }
}
