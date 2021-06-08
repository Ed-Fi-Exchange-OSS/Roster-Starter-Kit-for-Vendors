using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EdFi.Roster.ChangeQueries.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    public class ApiLogController : Controller
    {
        private readonly ApiLogService _apiLogService;

        public ApiLogController(ApiLogService apiLogService)
        {
            _apiLogService = apiLogService;
        }
        public async Task<IActionResult> Index()
        {
            var logs = (await _apiLogService.ReadAllLogsAsync()).OrderByDescending(l => l.LogDateTime);
            return View(logs);
        }

        public IActionResult ClearLogs()
        {
            _apiLogService.ClearLogs();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RawLog()
        {
            var logs = (await _apiLogService.ReadAllLogsAsync()).OrderByDescending(l => l.LogDateTime);
            return View("RawLog", JsonSerializer.Serialize(logs, options: new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
