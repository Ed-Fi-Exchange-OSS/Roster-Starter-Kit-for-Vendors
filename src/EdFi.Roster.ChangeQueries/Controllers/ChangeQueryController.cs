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
        private readonly SchoolService _schoolService;
        private readonly SectionService _sectionService;
        private readonly StaffService _staffService;

        public ChangeQueryController(ChangeQueryService changeQueryService
        , LocalEducationAgencyService localEducationAgencyService
        , SchoolService schoolService
        , SectionService sectionService
        , StaffService staffService)
        {
            _changeQueryService = changeQueryService;
            _localEducationAgencyService = localEducationAgencyService;
            _schoolService = schoolService;
            _sectionService = sectionService;
            _staffService = staffService;
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

        public async Task<IActionResult> SyncData()
        {
            var availableVersion = await _changeQueryService.GetAvailableVersionAsync();
            var currentVersions = await _changeQueryService.ReadCurrentVersionsForResourcesAsync();

            var responses = new List<DataSyncResponseModel>();

            var leaChangeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == ResourceTypes.LocalEducationAgencies);
            var leaMinVersion = leaChangeVersion?.ChangeVersion ?? 0;
            var leaResponse = await _localEducationAgencyService.RetrieveAndSyncLocalEducationAgencies(leaMinVersion, availableVersion);

            responses.Add(leaResponse);

            var schoolChangeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == ResourceTypes.Schools);
            var schoolMinVersion = schoolChangeVersion?.ChangeVersion ?? 0;
            var schoolResponse = await _schoolService.RetrieveAndSyncSchools(schoolMinVersion, availableVersion);

            responses.Add(schoolResponse);

            var sectionChangeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == ResourceTypes.Sections);
            var sectionMinVersion = sectionChangeVersion?.ChangeVersion ?? 0;
            var sectionResponse = await _sectionService.RetrieveAndSyncSections(sectionMinVersion, availableVersion);

            responses.Add(sectionResponse);

            var staffChangeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == ResourceTypes.Staff);
            var staffMinVersion = staffChangeVersion?.ChangeVersion ?? 0;
            var staffResponse = await _staffService.RetrieveAndSyncStaff(staffMinVersion, availableVersion);

            responses.Add(staffResponse);

            var changeQueryModel = new ChangeQueryViewModel
            {
                ChangeSummaryMessage = "Changes synced successfully.",
                SyncResponses = responses
            };
            return View("Index", changeQueryModel);
        }
    }
}
