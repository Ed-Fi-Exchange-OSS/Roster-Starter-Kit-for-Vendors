using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services;
using EdFi.Roster.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiSettingsService = EdFi.Common.ApiSettingsService;

namespace EdFi.Roster.ChangeQueries.Controllers
{
    public class EdFiApiSettings : Controller
    {
        private readonly BearerTokenService _bearerTokenService;
        private readonly ApiSettingsService _apiSettingsService;
        private readonly ChangeQueryService _changeQueryService;
        private readonly IDataService _dataService;

        public EdFiApiSettings(BearerTokenService bearerTokenService, ApiSettingsService apiSettingsService, ChangeQueryService changeQueryService, IDataService dataService)
        {
            _bearerTokenService = bearerTokenService;
            _apiSettingsService = apiSettingsService;
            _changeQueryService = changeQueryService;
            _dataService = dataService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ApiConnectionStatus status = ApiConnectionStatus.Success)
        {
            ViewData["ApiConnectionStatus"] = status switch
            {
                ApiConnectionStatus.NotConfigured => "Please connect to a sample ODS / API before proceeding.",
                ApiConnectionStatus.Error => "The application could not connect to the ODS / API. Please review your connection settings and ensure the ODS / API is running and accessible.",
                _ => ViewData["ApiConnectionStatus"]
            };
            var model = await _apiSettingsService.Read();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(string rootUrl, string key, string secret)
        {
            //save the settings
            var model = new ApiSettings{Key = key, RootUrl = rootUrl, Secret = secret};
            var testConnectionResult = await TestChangeQueryConnection(model);
            if (testConnectionResult.StatusCode != (int)HttpStatusCode.OK)
            {
                return testConnectionResult;
            }
            await _apiSettingsService.Save(model);

            DeleteResources();

            return new JsonResult(model);
        }

        private void DeleteResources()
        {
            _dataService.ClearRecords<RosterLocalEducationAgencyResource>();
            _dataService.ClearRecords<RosterSchoolResource>();
            _dataService.ClearRecords<RosterSectionResource>();
            _dataService.ClearRecords<RosterStaffResource>();
            _dataService.ClearRecords<RosterStudentResource>();
            _dataService.ClearRecords<ChangeQuery>();
            _dataService.ClearRecords<ApiLogEntry>();
        }

        [HttpPost]
        public async Task<IActionResult> TestConnection(string rootUrl, string key, string secret)
        {
            var apiSettings = new ApiSettings
            {
                RootUrl = rootUrl,
                Key = key,
                Secret = secret
            };

            return await TestChangeQueryConnection(apiSettings);
        }

        private async Task<ObjectResult> TestChangeQueryConnection(ApiSettings apiSettings)
        {
            var response = await _bearerTokenService.GetNewBearerTokenResponse(apiSettings);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return StatusCode((int) response.StatusCode,
                    "The connection was tested by attempting to authenticate with the information provided above, " +
                    "but the attempt failed. Please review the values above and confirm that " +
                    "if you navigate directly to the given URL in another browser window, that you " +
                    "arrive at the ODS / API's root JSON document including its " +
                    "version metadata. The attempt to access the /oauth/token authentication endpoint returned: " +
                    $"{(int) response.StatusCode} ({response.StatusCode}) {response.Data?.Error}");
            }

            var changeQueryResponse =
                await _changeQueryService.TestChangeQueryApiAsync(apiSettings, response.Data.AccessToken);

            if (changeQueryResponse.StatusCode != HttpStatusCode.OK)
            {
                return StatusCode((int) changeQueryResponse.StatusCode,
                    changeQueryResponse.StatusCode == HttpStatusCode.NotFound
                        ? "It looks like this ODS doesn't have Change Queries enabled. Review your Root URL and check that your ODS has Change Queries enabled."
                        : JsonConvert.SerializeObject(changeQueryResponse));
            }

            return StatusCode((int)response.StatusCode, JsonConvert.SerializeObject(response));
        }
    }
}
