using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EdFi.Roster.Explorer.ActionFilters;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using EdFi.Roster.Services;

namespace EdFi.Roster.Explorer.Controllers
{
    public class SchoolsController : Controller
    {
        private readonly SchoolService _schoolService;

        public SchoolsController(SchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IActionResult> Index()
        {
            //Read any saved Schools previously saved to be displayed
            var schools = await _schoolService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<School>>
            {
                FullDataSet = schools.ToList(),
                IsExtendedInfoAvailable = false
            });
        }

        [ValidateApiConnection]
        public async Task<IActionResult> LoadSchools()
        {
            var response = await _schoolService.GetAllSchoolsWithExtendedInfoAsync();
            await _schoolService.Save(response.FullDataSet);
            response.IsExtendedInfoAvailable = true;
            return View("Index", response);
        }
    }

}
