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
    public class SchoolService : ApiService<SchoolsApi, EdFiSchool, RosterSchoolResource>
    {
        public SchoolService(
            IDataService dataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade,
            ChangeQueryService changeQueryService)
            : base(dataService, responseHandleService, apiFacade, changeQueryService)
        {
        }

        protected override string ApiRoute => ApiRoutes.SchoolsResource;

        protected override string ResourceType => ResourceTypes.Schools;

        protected override async Task<ApiResponse<List<EdFiSchool>>> GetChangesAsync(SchoolsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.GetSchoolsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override async Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(SchoolsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.DeletesSchoolsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override string GetResourceId(EdFiSchool resource) => resource.Id;
    }
}
