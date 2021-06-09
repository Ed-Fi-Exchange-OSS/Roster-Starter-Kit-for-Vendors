using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Roster.ChangeQueries.Services;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    public class SyncedResourcesController : Controller
    {
        private readonly IResourceDataService _rosterDataService;

        public SyncedResourcesController(IResourceDataService rosterDataService)
            => _rosterDataService = rosterDataService;

        public async Task<IActionResult> LocalEducationAgencies()
            => View(await QueryAsync<RosterLocalEducationAgencyResource, EdFiLocalEducationAgency>());

        public async Task<IActionResult> Schools()
            => View(await QueryAsync<RosterSchoolResource, EdFiSchool>());

        public async Task<IActionResult> Staff()
            => View(await QueryAsync<RosterStaffResource, EdFiStaff>());

        public async Task<IActionResult> Students()
            => View(await QueryAsync<RosterStudentResource, EdFiStudent>());

        public async Task<IActionResult> Sections()
            => View(await QueryAsync<RosterSectionResource, EdFiSection>());

        private async Task<List<TApiModel>> QueryAsync<TEntityModel, TApiModel>()
            where TEntityModel : RosterDataRecord
        {
            var entities = await _rosterDataService.ReadAllAsync<TEntityModel>();
            var deserializedApiModels = entities.Select(lea => JsonConvert.DeserializeObject<TApiModel>(lea.Content)).ToList();

            return deserializedApiModels;
        }
    }
}
