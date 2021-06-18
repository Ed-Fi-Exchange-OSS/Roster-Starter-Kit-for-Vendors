using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;

namespace EdFi.Roster.Explorer.Services
{
    public class StudentService :
        ApiService<StudentsApi, Student, RosterStudentComposite>
    {
        public StudentService(IDataService rosterDataService, IResponseHandleService responseHandleService, IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        protected override string ApiRoute => ApiRoutes.StudentsComposite;

        protected override async Task<ApiResponse<List<Student>>> GetPageAsync(StudentsApi api, int offset, int limit)
            => await api.GetStudentsWithHttpInfoAsync(offset, limit);

        protected override string GetResourceId(Student resource) => resource.Id;
    }
}
