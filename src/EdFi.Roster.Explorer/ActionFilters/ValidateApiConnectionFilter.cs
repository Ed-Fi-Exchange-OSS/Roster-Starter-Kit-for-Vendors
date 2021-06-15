using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Explorer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Roster.Explorer.ActionFilters
{
    public class ValidateApiConnectionFilter : IAsyncActionFilter
    {
        private readonly ApiSettingsService _apiSettingsService;
        private readonly BearerTokenService _bearerTokenService;

        public ValidateApiConnectionFilter(ApiSettingsService apiSettingsService, BearerTokenService bearerTokenService)
        {
            _apiSettingsService = apiSettingsService;
            _bearerTokenService = bearerTokenService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var apiSettings = await _apiSettingsService.Read();
            var routeDictionary = new RouteValueDictionary
                {{"controller", "EdFiApiSettings"}, {"action", "Index"}, {"status", ApiConnectionStatus.Success}};

            if (string.IsNullOrEmpty(apiSettings.RootUrl))
            {
                routeDictionary["status"] = ApiConnectionStatus.NotConfigured;
                context.Result = new RedirectToRouteResult(routeDictionary);
                return;
            }

            try
            {
                await _bearerTokenService.GetNewBearerTokenResponse(apiSettings);
            }
            catch
            {
                routeDictionary["status"] = ApiConnectionStatus.Error;
                context.Result = new RedirectToRouteResult(routeDictionary);
                return;
            }

            await next();
        }
    }
}
