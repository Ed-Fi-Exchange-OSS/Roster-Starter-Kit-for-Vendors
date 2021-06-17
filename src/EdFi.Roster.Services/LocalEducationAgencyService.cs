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
    public class LocalEducationAgencyService : ApiService
    {
        private readonly IDataService _rosterDataService;

        public LocalEducationAgencyService(
            IDataService rosterDataService,
            IResponseHandleService responseHandleService,
            IApiFacade apiFacade)
            : base(responseHandleService, apiFacade)
        {
            _rosterDataService = rosterDataService;
        }

        public async Task Save(List<LocalEducationAgency> localEducationAgencies)
        {
            var leas = localEducationAgencies.Select(lea =>
                new RosterLocalEducationAgencyComposite {Content = JsonConvert.SerializeObject(lea), ResourceId = lea.Id}).ToList();

             await _rosterDataService.SaveAsync(leas);
        }

        public async Task<List<LocalEducationAgency>> ReadAllAsync()
        {
            var leas = await _rosterDataService.ReadAllAsync<RosterLocalEducationAgencyComposite>();
            return leas.Select(lea => JsonConvert.DeserializeObject<LocalEducationAgency>(lea.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<LocalEducationAgency>>> GetAllLocalEducationAgenciesWithExtendedInfoAsync()
        {
            return await GetAllResourcesWithExtendedInfoAsync<LocalEducationAgenciesApi, LocalEducationAgency>(
                ApiRoutes.LocalEducationAgenciesComposite,
                async (api, offset, limit) =>
                    await api.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit));
        }
    }
}

