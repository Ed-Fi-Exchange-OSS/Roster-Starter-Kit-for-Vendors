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
    public class StudentService : ApiService<StudentsApi, EdFiStudent, RosterStudentResource>
    {
        public StudentService(
            IDataService dataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade,
            ChangeQueryService changeQueryService)
            : base(dataService, responseHandleService, apiFacade, changeQueryService)
        {
        }

        protected override string ApiRoute => ApiRoutes.StudentsResource;

        protected override string ResourceType => ResourceTypes.Students;

        protected override async Task<ApiResponse<List<EdFiStudent>>> GetChangesAsync(StudentsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.GetStudentsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override async Task<ApiResponse<List<DeletedResource>>> GetDeletionsAsync(StudentsApi api, int offset, int limit, int minChangeVersion, int maxChangeVersion)
            => await api.DeletesStudentsWithHttpInfoAsync(offset, limit, minChangeVersion, maxChangeVersion);

        protected override string GetResourceId(EdFiStudent resource) => resource.Id;
    }
}
