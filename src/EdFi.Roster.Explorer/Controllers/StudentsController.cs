using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                FullDataSet = students.ToList(),
                IsExtendedInfoAvailable = false
            });
        }
        public async Task<IActionResult> LoadStudents()
        {
            var response = await _studentService.GetAllStudentsWithExtendedInfoAsync();
            await _studentService.Save(response.FullDataSet);
            response.IsExtendedInfoAvailable = true;
            return View("Index", response);
        }
    }
}
