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
        {
            _rosterDataService = rosterDataService;
        }

        public async Task<IActionResult> LocalEducationAgencies()
        {
            return View(new ExtendedInfoResponse<List<LocalEducationAgency>>
            {
                FullDataSet = await QueryAsync<RosterLocalEducationAgencyComposite, LocalEducationAgency>(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Schools()
        {
            return View(new ExtendedInfoResponse<List<School>>
            {
                FullDataSet = await QueryAsync<RosterSchoolComposite, School>(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Staff()
        {
            return View(new ExtendedInfoResponse<List<Staff>>
            {
                FullDataSet = await QueryAsync<RosterStaffComposite, Staff>(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Students()
        {
            return View(new ExtendedInfoResponse<List<Student>>
            {
                FullDataSet = await QueryAsync<RosterStudentComposite, Student>(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Sections()
        {
            return View(new ExtendedInfoResponse<List<Section>>
            {
                FullDataSet = await QueryAsync<RosterSectionComposite, Section>(),
                IsExtendedInfoAvailable = false
            });
        }

        private async Task<List<TApiModel>> QueryAsync<TEntityModel, TApiModel>()
            where TEntityModel : RosterDataRecord
        {
            var entities = await _rosterDataService.ReadAllAsync<TEntityModel>();
            var deserializedApiModels = entities.Select(lea => JsonConvert.DeserializeObject<TApiModel>(lea.Content)).ToList();

            return deserializedApiModels;
        }
    }
}
