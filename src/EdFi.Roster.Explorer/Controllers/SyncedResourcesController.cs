using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using EdFi.Roster.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    public class SyncedResourcesController : Controller
    {
        private readonly LocalEducationAgencyService _localEducationAgencyService;
        private readonly SchoolService _schoolService;
        private readonly StaffService _staffService;
        private readonly StudentService _studentService;
        private readonly SectionService _sectionsService;

        public SyncedResourcesController(
            LocalEducationAgencyService localEducationAgencyService,
            SchoolService schoolService,
            StaffService staffService,
            StudentService studentService,
            SectionService sectionService)
        {
            _localEducationAgencyService = localEducationAgencyService;
            _schoolService = schoolService;
            _staffService = staffService;
            _studentService = studentService;
            _sectionsService = sectionService;
        }

        public async Task<IActionResult> LocalEducationAgencies()
        {
            var leaList = await _localEducationAgencyService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<LocalEducationAgency>>
            {
                FullDataSet = leaList.ToList(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Schools()
        {
            var schools = await _schoolService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<School>>
            {
                FullDataSet = schools.ToList(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Staff()
        {
            var staffList = await _staffService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<Staff>>
            {
                FullDataSet = staffList.ToList(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Students()
        {
            var students = await _studentService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<Student>>
            {
                FullDataSet = students.ToList(),
                IsExtendedInfoAvailable = false
            });
        }

        public async Task<IActionResult> Sections()
        {
            var sections = await _sectionsService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<Section>>
            {
                FullDataSet = sections.ToList(),
                IsExtendedInfoAvailable = false
            });
        }
    }
}
