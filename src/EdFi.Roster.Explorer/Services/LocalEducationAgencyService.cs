using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.Services
{
    public class LocalEducationAgencyService :
        ApiService<LocalEducationAgenciesApi, LocalEducationAgency, RosterLocalEducationAgencyComposite>
    {
        public LocalEducationAgencyService(IDataService rosterDataService, IResponseHandleService responseHandleService, IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        protected override string ApiRoute => ApiRoutes.LocalEducationAgenciesComposite;

        protected override async Task<ApiResponse<List<LocalEducationAgency>>> GetPageAsync(LocalEducationAgenciesApi api, int offset, int limit)
            => await api.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit);

        protected override string GetResourceId(LocalEducationAgency resource) => resource.Id;
    }
}

