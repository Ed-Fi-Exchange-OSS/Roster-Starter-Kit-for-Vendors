using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    public class ChangeQueryController : Controller
    {
        private readonly ChangeQueryService _changeQueryService;
        private readonly LocalEducationAgencyService _localEducationAgencyService;

        public ChangeQueryController(ChangeQueryService changeQueryService
        , LocalEducationAgencyService localEducationAgencyService)
        {
            _changeQueryService = changeQueryService;
            _localEducationAgencyService = localEducationAgencyService;
        }

        public async Task<IActionResult> Index()
        {
            var availableVersion = await _changeQueryService.GetAvailableVersionAsync();
            var currentVersions = await _changeQueryService.ReadCurrentVersionsForResourcesAsync();

            var changeQueryModel = new ChangeQueryViewModel
            {
                ChangeSummaryMessage = "No pending changes to sync. Please come back later for updates."
            };


            if (currentVersions.Count == 0 || currentVersions.Select(x => x.ChangeVersion).Any(x => x < availableVersion))
            {
                changeQueryModel.ChangeSummaryMessage = "There are pending changes to sync. Please click the Sync button to update your records.";
            }

            return View(changeQueryModel);
        }

        public async Task<IActionResult> SyncData()
        {
            var availableVersion = await _changeQueryService.GetAvailableVersionAsync();
            var currentVersions = await _changeQueryService.ReadCurrentVersionsForResourcesAsync();

            var responses = new List<DataSyncResponseModel>();

            var leaChangeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == ResourceTypes.LocalEducationAgencies);
            var minVersion = leaChangeVersion?.ChangeVersion ?? 0;
            var laeResponse = await _localEducationAgencyService.RetrieveAndSyncLocalEducationAgencies(minVersion, availableVersion);
            
            responses.Add(laeResponse);

            var changeQueryModel = new ChangeQueryViewModel
            {
                ChangeSummaryMessage = "Changes synced successfully.",
                SyncResponses = responses
            };
            return View("Index", changeQueryModel);
        }
    }
}
