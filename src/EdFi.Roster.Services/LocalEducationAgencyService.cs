using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;

namespace EdFi.Roster.Services
{
    public class LocalEducationAgencyService :
        ApiService<LocalEducationAgenciesApi, LocalEducationAgency, RosterLocalEducationAgencyComposite>
    {
        public LocalEducationAgencyService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        public async Task<ExtendedInfoResponse<List<LocalEducationAgency>>> GetAllLocalEducationAgenciesWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync(
                ApiRoutes.LocalEducationAgenciesComposite,
                async (api, offset, limit) =>
                    await api.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit));
        }

        protected override string GetResourceId(LocalEducationAgency resource) => resource.Id;
    }
}

