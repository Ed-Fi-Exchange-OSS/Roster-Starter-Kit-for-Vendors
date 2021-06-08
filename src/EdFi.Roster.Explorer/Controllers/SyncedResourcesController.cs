using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using EdFi.Roster.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EdFi.Roster.Explorer.Controllers
{
    public class SyncedResourcesController : Controller
    {
        private readonly IRosterDataService _rosterDataService;

        public SyncedResourcesController(IRosterDataService rosterDataService)
            => _rosterDataService = rosterDataService;

        public async Task<IActionResult> LocalEducationAgencies()
            => View(await QueryAsync<RosterLocalEducationAgencyComposite, LocalEducationAgency>());

        public async Task<IActionResult> Schools()
            => View(await QueryAsync<RosterSchoolComposite, School>());

        public async Task<IActionResult> Staff()
            => View(await QueryAsync<RosterStaffComposite, Staff>());

        public async Task<IActionResult> Students()
            => View(await QueryAsync<RosterStudentComposite, Student>());

        public async Task<IActionResult> Sections()
            => View(await QueryAsync<RosterSectionComposite, Section>());

        private async Task<List<TApiModel>> QueryAsync<TEntityModel, TApiModel>()
            where TEntityModel : RosterDataRecord
        {
            var entities = await _rosterDataService.ReadAllAsync<TEntityModel>();
            var deserializedApiModels = entities.Select(lea => JsonConvert.DeserializeObject<TApiModel>(lea.Content)).ToList();

            return deserializedApiModels;
        }
    }
}
