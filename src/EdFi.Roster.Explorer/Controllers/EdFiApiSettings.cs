using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
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
        public async Task<IActionResult> Index()
        {
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
            return StatusCode((int)response.StatusCode, JsonConvert.SerializeObject(response));
        }
    }
}
