using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;

namespace EdFi.Roster.Services
{
    public class StudentService :
        ApiService<StudentsApi, Student, RosterStudentComposite>
    {
        public StudentService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(rosterDataService, responseHandleService, apiFacade)
        {
        }

        public async Task<ExtendedInfoResponse<List<Student>>> GetAllStudentsWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync(
                ApiRoutes.StudentsComposite,
                async (api, offset, limit) =>
                    await api.GetStudentsWithHttpInfoAsync(offset, limit));
        }

        protected override string GetResourceId(Student resource) => resource.Id;
    }
}
