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
    public class SectionService : ApiService<SectionsApi, EdFiSection, RosterSectionResource>
    {
        public SectionService(IDataService dataService, IResponseHandleService responseHandleService, IApiFacade apiFacade, ChangeQueryService changeQueryService)
            : base(dataService, responseHandleService, apiFacade, changeQueryService)
        {
        }

        protected override string ApiRoute => ApiRoutes.SectionsResource;

        protected override string ResourceType => ResourceTypes.Sections;

        protected override async Task<ApiResponse<List<EdFiSection>>> GetChangesAsync(SectionsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.GetSectionsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override async Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(SectionsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.DeletesSectionsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override string GetResourceId(EdFiSection resource) => resource.Id;
    }
}
