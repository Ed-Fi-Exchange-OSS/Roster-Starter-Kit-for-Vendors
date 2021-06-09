using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace EdFi.Common
{
    public interface IApiFacade
    {
        Task<T> GetApiClassInstance<T>(bool refreshToken = false);
        Uri BuildResponseUri(string apiRoute, int offset, int limit);
    }

    public class ApiFacade : IApiFacade
    {
        private readonly IConfigurationService _configurationService;
        private string BasePath { get; set; }

        public ApiFacade(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<T> GetApiClassInstance<T>(bool refreshToken = false)
        {
            var apiConfiguration = await _configurationService.ApiConfiguration(refreshToken);
            BasePath = apiConfiguration.BasePath;
            return (T)Activator.CreateInstance(typeof(T), apiConfiguration);
        }

        public Uri BuildResponseUri(string apiRoute, int offset, int limit)
        {
            var url = $"{BasePath}/{apiRoute}";
            var queryParams = new Dictionary<string, string> { { "offset", offset.ToString() }, { "limit", limit.ToString() } };

            return new Uri(QueryHelpers.AddQueryString(url, queryParams), UriKind.Absolute);
        }
    }
}
