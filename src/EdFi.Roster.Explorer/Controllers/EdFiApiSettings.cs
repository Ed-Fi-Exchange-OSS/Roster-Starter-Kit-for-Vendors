using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Explorer.Models;
using EdFi.Roster.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiSettingsService = EdFi.Common.ApiSettingsService;

namespace EdFi.Roster.Explorer.Controllers
{
    public class EdFiApiSettings : Controller
    {
        private readonly BearerTokenService _bearerTokenService;
        private readonly ApiSettingsService _apiSettingsService;

        public EdFiApiSettings(BearerTokenService bearerTokenService, ApiSettingsService apiSettingsService)
        {
            _bearerTokenService = bearerTokenService;
            _apiSettingsService = apiSettingsService;
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
            var model = new ApiSettings { Key = key, RootUrl = rootUrl, Secret = secret };
            var testConnectionResult = await TestApiConnection(model);
            if (testConnectionResult.StatusCode != (int)HttpStatusCode.OK)
            {
                return testConnectionResult;
            }
            await _apiSettingsService.Save(model);
            return new JsonResult(model);
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

            return await TestApiConnection(apiSettings);
        }

        private async Task<ObjectResult> TestApiConnection(ApiSettings apiSettings)
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

            return StatusCode((int)response.StatusCode, JsonConvert.SerializeObject(response));
        }
    }
}
