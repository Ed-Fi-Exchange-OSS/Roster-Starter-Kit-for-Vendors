using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Roster.Explorer.ActionFilters;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using EdFi.Roster.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;
        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.ReadAllAsync();
            return View(new ExtendedInfoResponse<List<Student>>
            {
                FullDataSet = students,
                IsExtendedInfoAvailable = false
            });
        }

        [ValidateApiConnection]
        public async Task<IActionResult> LoadStudents()
        {
            var response = await _studentService.GetAllResourcesWithExtendedInfoAsync();
            await _studentService.Save(response.FullDataSet);
            response.IsExtendedInfoAvailable = true;
            return View("Index", response);
        }
    }
}
