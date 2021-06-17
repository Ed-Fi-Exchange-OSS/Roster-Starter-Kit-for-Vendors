using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;

namespace EdFi.Roster.Services
{
    public class SchoolService :
        ApiService<SchoolsApi, School, RosterSchoolComposite>
    {
        public SchoolService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        public async Task<ExtendedInfoResponse<List<School>>> GetAllSchoolsWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync(
                ApiRoutes.SchoolsComposite,
                async (api, offset, limit) =>
                    await api.GetSchoolsWithHttpInfoAsync(offset, limit));
        }

        protected override string GetResourceId(School resource) => resource.Id;
    }
}
