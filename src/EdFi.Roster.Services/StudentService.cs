using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public class StudentService : ApiService
    {
        private readonly IDataService _rosterDataService;

        public StudentService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(responseHandleService, apiFacade)
        {
            _rosterDataService = rosterDataService;
        }

        public async Task<List<Student>> ReadAllAsync()
        {
            var students = await _rosterDataService.ReadAllAsync<RosterStudentComposite>();
            return students.Select(st => JsonConvert.DeserializeObject<Student>(st.Content)).ToList();
        }

        public async Task Save(List<Student> students)
        {
            var studentList = students
                .Select(student => new RosterStudentComposite { Content = JsonConvert.SerializeObject(student), ResourceId = student.Id}).ToList();
            await _rosterDataService.SaveAsync(studentList);
        }

        public async Task<ExtendedInfoResponse<List<Student>>> GetAllStudentsWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync<StudentsApi, Student>(
                ApiRoutes.StudentsComposite,
                async (api, offset, limit) =>
                    await api.GetStudentsWithHttpInfoAsync(offset, limit));
        }
    }
}
