using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.Sdk.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace EdFi.Roster.ChangeQueries.ActionFilters
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
                var bearerTokenResponse = await _bearerTokenService.GetNewBearerTokenResponse(apiSettings);

                if (!string.IsNullOrEmpty(bearerTokenResponse.Data.Error) || bearerTokenResponse.StatusCode != HttpStatusCode.OK)
                    throw new ApiException((int) bearerTokenResponse.StatusCode, bearerTokenResponse.Data.Error);
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
