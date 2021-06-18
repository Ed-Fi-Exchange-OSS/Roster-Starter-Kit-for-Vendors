using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Roster.Explorer.ActionFilters;
using EdFi.Roster.Explorer.Services;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    public class LocalEducationAgenciesController : Controller
    {
        private readonly LocalEducationAgencyService _localEducationAgencyService;

        public LocalEducationAgenciesController(LocalEducationAgencyService localEducationAgencyService)
        {
            _localEducationAgencyService = localEducationAgencyService;
        }

        public async Task<IActionResult> Index()
        {
            //Read any saved LEAs to be displayed
            var leaList = await _localEducationAgencyService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<LocalEducationAgency>>
            {
                FullDataSet = leaList,
                IsExtendedInfoAvailable = false
            }); 
        }

        [ValidateApiConnection]
        public async Task<IActionResult> LoadLeas()
        {
            var response = await _localEducationAgencyService.GetAllResourcesWithExtendedInfoAsync();
            await _localEducationAgencyService.Save(response.FullDataSet);
            response.IsExtendedInfoAvailable = true;
            return View("Index", response);
        }
    }
}
