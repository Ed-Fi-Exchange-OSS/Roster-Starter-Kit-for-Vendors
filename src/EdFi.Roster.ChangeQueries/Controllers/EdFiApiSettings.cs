using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
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

        public EdFiApiSettings(BearerTokenService bearerTokenService, ApiSettingsService apiSettingsService, ChangeQueryService changeQueryService)
        {
            _bearerTokenService = bearerTokenService;
            _apiSettingsService = apiSettingsService;
            _changeQueryService = changeQueryService;
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
            var model = new ApiSettings{Key = key, RootUrl = rootUrl, Secret = secret};
            var testConnectionResult = (ObjectResult)await TestChangeQueryConnection(model);
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

            return await TestChangeQueryConnection(apiSettings);
        }

        private async Task<IActionResult> TestChangeQueryConnection(ApiSettings apiSettings)
        {
            var response = await _bearerTokenService.GetNewBearerTokenResponse(apiSettings);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var changeQueryResponse =
                    await _changeQueryService.TestChangeQueryAPIAsync(apiSettings, response.Data.AccessToken);

                if (changeQueryResponse.StatusCode != HttpStatusCode.OK)
                {
                    return StatusCode((int) changeQueryResponse.StatusCode,
                        changeQueryResponse.StatusCode == HttpStatusCode.NotFound ?
                        "It looks like this ODS doesn't have Change Queries enabled. Review your Root URL and check that your ODS has Change Queries enabled."
                        : JsonConvert.SerializeObject(changeQueryResponse));
                }
            }

            return StatusCode((int) response.StatusCode, JsonConvert.SerializeObject(response));
        }
    }
}
