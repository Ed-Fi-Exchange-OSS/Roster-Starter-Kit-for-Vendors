using System.Threading.Tasks;
using EdFi.Roster.Models;
using EdFi.Roster.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ApiSettingsService = EdFi.Roster.Services.ApiSettingsService;

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
            var model = new ApiSettings{Key = key, RootUrl = rootUrl, Secret = secret};
            await _apiSettingsService.Save(model);
            return new JsonResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> TestConnection(string rootUrl, string key, string secret)
        {
            var response = await _bearerTokenService.GetNewBearerTokenResponse(new ApiSettings
            {
                RootUrl = rootUrl, 
                Key = key, 
                Secret = secret
            });

            var content = JsonConvert.SerializeObject(response);
            return Content(content, "application/json");
        }
    }
}
