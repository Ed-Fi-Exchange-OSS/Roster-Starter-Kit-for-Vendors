using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    public class ChangeQueryController : Controller
    {
        private readonly ChangeQueryService _changeQueryService;

        public ChangeQueryController(ChangeQueryService changeQueryService)
        {
            _changeQueryService = changeQueryService;
        }

        public async Task<IActionResult> Index()
        {
            var availableVersion = await _changeQueryService.GetAvailableVersionAsync();
            var currentVersions = await _changeQueryService.ReadCurrentVersionsForResourcesAsync();

            var changeQueryModel = new ChangeQueryViewModel
            {
                ChangeSummaryMessage = "No pending changes to sync. Please come back later for updates."
            };


            if (currentVersions.Select(x => x.ChangeVersion).Any(x => x < availableVersion))
            {
                changeQueryModel.ChangeSummaryMessage = "There are pending changes to sync. Please click the Sync button to update your records.";
            }

            return View(changeQueryModel);
        }
    }
}
