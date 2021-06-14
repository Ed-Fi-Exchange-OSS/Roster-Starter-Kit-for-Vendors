using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Roster.Sdk.Client;
using Microsoft.AspNetCore.WebUtilities;

namespace EdFi.Common
{
    public interface IApiFacade
    {
        Task<TApiAccessor> GetApiClassInstance<TApiAccessor>(bool refreshToken = false, bool isChangeQueries = false)
            where TApiAccessor : IApiAccessor;

        Uri BuildResponseUri(string apiRoute, Dictionary<string, string> queryParameters = null);
    }

    public class ApiFacade : IApiFacade
    {
        private readonly IConfigurationService _configurationService;
        private string BasePath { get; set; }

        public ApiFacade(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<T> GetApiClassInstance<T>(bool refreshToken = false, bool isChangeQueries = false)
            where T : IApiAccessor
        {
            var apiConfiguration = await _configurationService.ApiConfiguration(refreshToken, isChangeQueries);
            BasePath = apiConfiguration.BasePath;
            return (T)Activator.CreateInstance(typeof(T), apiConfiguration);
        }

        public Uri BuildResponseUri(string apiRoute, Dictionary<string, string> queryParameters = null)
        {
            var url = $"{BasePath}/{apiRoute}";
            return queryParameters != null && queryParameters.Any()
                ? new Uri(QueryHelpers.AddQueryString(url, queryParameters), UriKind.Absolute)
                : new Uri(url, UriKind.Absolute);
        }
    }
}
