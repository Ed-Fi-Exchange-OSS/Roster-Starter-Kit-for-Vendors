using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Roster.Services
{
    public class SchoolService :
        ApiService<SchoolsApi, School, RosterSchoolComposite>
    {
        public SchoolService(IDataService rosterDataService, IResponseHandleService responseHandleService, IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        protected override string ApiRoute => ApiRoutes.SchoolsComposite;

        protected override async Task<ApiResponse<List<School>>> GetPageAsync(SchoolsApi api, int offset, int limit)
            => await api.GetSchoolsWithHttpInfoAsync(offset, limit);

        protected override string GetResourceId(School resource) => resource.Id;
    }
}
