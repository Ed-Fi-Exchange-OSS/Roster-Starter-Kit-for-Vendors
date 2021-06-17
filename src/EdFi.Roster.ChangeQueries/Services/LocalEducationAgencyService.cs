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
    public class LocalEducationAgencyService : ApiService<LocalEducationAgenciesApi, EdFiLocalEducationAgency, RosterLocalEducationAgencyResource>
    {
        public LocalEducationAgencyService(
            IDataService dataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade,
            ChangeQueryService changeQueryService)
            : base(dataService, responseHandleService, apiFacade, changeQueryService)
        {
        }

        protected override string ApiRoute => ApiRoutes.LocalEducationAgenciesResource;

        protected override string ResourceType => ResourceTypes.LocalEducationAgencies;

        protected override async Task<ApiResponse<List<EdFiLocalEducationAgency>>> GetChangesAsync(LocalEducationAgenciesApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override async Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(LocalEducationAgenciesApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.DeletesLocalEducationAgenciesWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override string GetResourceId(EdFiLocalEducationAgency resource) => resource.Id;
    }
}
