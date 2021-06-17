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
    public class SectionService : ApiService
    {
        private readonly IDataService _rosterDataService;

        public SectionService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(responseHandleService, apiFacade)
        {
            _rosterDataService = rosterDataService;
        }

        public async Task Save(List<Section> sections)
        {
            var sectionsList = sections.Select(section =>
                new RosterSectionComposite { Content = JsonConvert.SerializeObject(section), ResourceId = section.Id}).ToList();
            await _rosterDataService.SaveAsync(sectionsList);
        }

        public async Task<List<Section>> ReadAllAsync()
        {
            var sections = await _rosterDataService.ReadAllAsync<RosterSectionComposite>();
            return sections.Select(section => JsonConvert.DeserializeObject<Section>(section.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<Section>>> GetAllSectionsWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync<SectionsApi, Section>(
                ApiRoutes.SectionsComposite,
                async (api, offset, limit) =>
                    await api.GetSectionsWithHttpInfoAsync(offset, limit));
        }
    }
}
