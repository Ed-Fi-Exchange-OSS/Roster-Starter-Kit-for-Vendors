using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EdFi.Roster.ChangeQueries.ActionFilters;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    [ValidateApiConnection]
    public class ChangeQueryController : Controller
    {
        private readonly ChangeQueryService _changeQueryService;
        private readonly LocalEducationAgencyService _localEducationAgencyService;
        private readonly StudentService _studentService;
        private readonly SchoolService _schoolService;
        private readonly SectionService _sectionService;
        private readonly StaffService _staffService;

        public ChangeQueryController(ChangeQueryService changeQueryService
        , LocalEducationAgencyService localEducationAgencyService
        , StudentService studentService
        , SchoolService schoolService
        , SectionService sectionService
        , StaffService staffService)
        {
            _changeQueryService = changeQueryService;
            _localEducationAgencyService = localEducationAgencyService;
            _studentService = studentService;
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
                HasPendingChanges = false,
                ChangeSummaryMessage = "No pending changes to sync. Please come back later for updates."
            };


            if (currentVersions.Count < ResourceTypes.ResourceTypeCount() || currentVersions.Select(x => x.ChangeVersion).Any(x => x < availableVersion))
            {
                changeQueryModel.HasPendingChanges = true;
                changeQueryModel.ChangeSummaryMessage = "There are pending changes to sync. Please click the Sync button to update your records.";
            }

            return View(changeQueryModel);
        }

        public async Task<IActionResult> SyncData()
        {
            var availableVersion = await _changeQueryService.GetAvailableVersionAsync();
            var responses = new List<DataSyncResponseModel>
            {
                await RunRetrieveAndSyncService(ResourceTypes.LocalEducationAgencies, availableVersion),
                await RunRetrieveAndSyncService(ResourceTypes.Schools, availableVersion),
                await RunRetrieveAndSyncService(ResourceTypes.Staff, availableVersion),
                await RunRetrieveAndSyncService(ResourceTypes.Students, availableVersion),
                await RunRetrieveAndSyncService(ResourceTypes.Sections, availableVersion)
            };

            var changeQueryModel = new ChangeQueryViewModel
            {
                HasPendingChanges = false,
                ChangeSummaryMessage = "Please review the sync status for individual resources below.",
                SyncResponses = responses
            };
            return View("Index", changeQueryModel);
        }

        private async Task<DataSyncResponseModel> RunRetrieveAndSyncService(string resourceType, long availableVersion)
        {
            var noChangesMessage = "No pending changes to sync for ";
            var currentVersions = await _changeQueryService.ReadCurrentVersionsForResourcesAsync();
            var changeVersion =
                currentVersions.SingleOrDefault(x => x.ResourceType == resourceType);
            var minVersion = changeVersion?.ChangeVersion ?? 0;

            if (minVersion >= availableVersion)
            {
                return new DataSyncResponseModel
                {
                    ResourceName = $"{noChangesMessage}{resourceType}"
                };
            }

            return resourceType switch
            {
                ResourceTypes.LocalEducationAgencies =>
                    await _localEducationAgencyService.RetrieveAndSyncResources(minVersion,
                        availableVersion),
                ResourceTypes.Schools => await _schoolService.RetrieveAndSyncResources(minVersion, availableVersion),
                ResourceTypes.Staff => await _staffService.RetrieveAndSyncResources(minVersion, availableVersion),
                ResourceTypes.Students => await _studentService.RetrieveAndSyncResources(minVersion, availableVersion),
                ResourceTypes.Sections => await _sectionService.RetrieveAndSyncResources(minVersion, availableVersion),
                _ => throw new Exception($"Cannot attempt sync for unexpected resource type: {resourceType}")
            };
        }
    }
}
