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
    public class SchoolService : ApiService
    {
        private readonly IDataService _rosterDataService;

        public SchoolService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(responseHandleService, apiFacade)
        {
            _rosterDataService = rosterDataService;
        }

        public async Task Save(List<School> schools)
        {
            var schoolList = schools.Select(school =>
                new RosterSchoolComposite {Content = JsonConvert.SerializeObject(school), ResourceId = school.Id}).ToList();

            await _rosterDataService.SaveAsync(schoolList);
        }

        public async Task<List<School>> ReadAllAsync()
        {
            var schools = await _rosterDataService.ReadAllAsync<RosterSchoolComposite>();
            return schools.Select(school => JsonConvert.DeserializeObject<School>(school.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<School>>> GetAllSchoolsWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync<SchoolsApi, School>(
                ApiRoutes.SchoolsComposite,
                async (api, offset, limit) =>
                    await api.GetSchoolsWithHttpInfoAsync(offset, limit));
        }
    }
}
