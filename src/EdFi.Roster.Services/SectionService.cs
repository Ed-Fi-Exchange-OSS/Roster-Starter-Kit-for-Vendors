using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.Services
{
    public class SectionService :
        ApiService<SectionsApi, Section, RosterSectionComposite>
    {
        public SectionService(IDataService rosterDataService, IResponseHandleService responseHandleService, IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        protected override string ApiRoute => ApiRoutes.SectionsComposite;

        protected override async Task<ApiResponse<List<Section>>> GetPageAsync(SectionsApi api, int offset, int limit)
            => await api.GetSectionsWithHttpInfoAsync(offset, limit);

        protected override string GetResourceId(Section resource) => resource.Id;
    }
}
