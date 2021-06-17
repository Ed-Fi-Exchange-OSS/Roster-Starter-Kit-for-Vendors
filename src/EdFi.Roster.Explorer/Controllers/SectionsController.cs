using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Roster.Explorer.ActionFilters;
using EdFi.Roster.Explorer.Services;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    public class SectionsController : Controller
    {
        private readonly SectionService _sectionsService;

        public SectionsController(SectionService sectionService)
        {
            _sectionsService = sectionService;
        }
        public async Task<IActionResult> Index()
        {
            var sections = await _sectionsService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<Section>>
            {
                FullDataSet = sections,
                IsExtendedInfoAvailable = false
            });
        }

        [ValidateApiConnection]
        public async Task<IActionResult> LoadSections()
        {
            var response = await _sectionsService.GetAllResourcesWithExtendedInfoAsync();
            await _sectionsService.Save(response.FullDataSet);
            response.IsExtendedInfoAvailable = true;
            return View("Index", response);
        }
    }
}
