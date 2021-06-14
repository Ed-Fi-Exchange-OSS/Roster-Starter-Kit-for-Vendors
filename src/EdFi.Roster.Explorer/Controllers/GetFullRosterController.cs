using System.Threading.Tasks;
using EdFi.Roster.Explorer.ActionFilters;
using EdFi.Roster.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    [ValidateApiConnection]
    public class GetFullRosterController : Controller
    {
        private readonly LocalEducationAgencyService _localEducationAgencyService;
        private readonly SchoolService _schoolService;
        private readonly SectionService _sectionService;
        private readonly StaffService _staffService;
        private readonly StudentService _studentService;

        public GetFullRosterController(LocalEducationAgencyService localEducationAgencyService
               , SchoolService schoolService
               , SectionService sectionService
               , StaffService staffService
               , StudentService studentService)
        {
            _localEducationAgencyService = localEducationAgencyService;
            _schoolService = schoolService;
            _sectionService = sectionService;
            _staffService = staffService;
            _studentService = studentService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("GetLeasAsync")]
        public async Task<IActionResult> GetLeasAsync()
        { 
            var response = await _localEducationAgencyService.GetAllLocalEducationAgenciesWithExtendedInfoAsync();
            await _localEducationAgencyService.Save(response.FullDataSet);
            return Ok(response.GeneralInfo);
        }

        [HttpGet("GetSchoolsAsync")]
        public async Task<IActionResult> GetSchoolsAsync()
        {
            var response = await _schoolService.GetAllSchoolsWithExtendedInfoAsync();
            await _schoolService.Save(response.FullDataSet);
            return Ok(response.GeneralInfo);
        }

        [HttpGet("GetSectionsAsync")]
        public async Task<IActionResult> GetSectionsAsync()
        {
            var response = await _sectionService.GetAllSectionsWithExtendedInfoAsync();
            await _sectionService.Save(response.FullDataSet);
            return Ok(response.GeneralInfo);
        }

        [HttpGet("GetStaffAsync")]
        public async Task<IActionResult> GetStaffAsync()
        {
            var response = await _staffService.GetAllStaffWithExtendedInfoAsync();
            await _staffService.Save(response.FullDataSet);
            return Ok(response.GeneralInfo);
        }

        [HttpGet("GetStudentsAsync")]
        public async Task<IActionResult> GetStudentsAsync()
        {
            var response = await _studentService.GetAllStudentsWithExtendedInfoAsync();
            await _studentService.Save(response.FullDataSet);
            return Ok(response.GeneralInfo);
        }

    }
}
