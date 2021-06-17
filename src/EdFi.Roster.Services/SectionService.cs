using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;

namespace EdFi.Roster.Services
{
    public class SectionService :
        ApiService<SectionsApi, Section, RosterSectionComposite>
    {
        public SectionService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        public async Task<ExtendedInfoResponse<List<Section>>> GetAllResourcesWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync(
                ApiRoutes.SectionsComposite,
                async (api, offset, limit) =>
                    await api.GetSectionsWithHttpInfoAsync(offset, limit));
        }

        protected override string GetResourceId(Section resource) => resource.Id;
    }
}
