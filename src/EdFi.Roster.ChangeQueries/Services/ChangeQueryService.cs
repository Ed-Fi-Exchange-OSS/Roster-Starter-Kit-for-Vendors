using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.ChangeQueries;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.ChangeQueries;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class ChangeQueryService
    {
        private readonly IDataService _dataService;
        private readonly IApiFacade _apiFacade;
        private readonly IResponseHandleService _responseHandleService;

        public ChangeQueryService(IDataService dataService
            , IApiFacade apiFacade
            , IResponseHandleService responseHandleService)
        {
            _dataService = dataService;
            _apiFacade = apiFacade;
            _responseHandleService = responseHandleService;
        }

        public async Task Save(long availableChangeVersion, string resourceType)
        {
            var changeQuery = new ChangeQuery
            {
                ChangeVersion = availableChangeVersion,
                ResourceType = resourceType
            };

            await _dataService.SaveAsync(changeQuery);
        }

        public async Task<long> GetAvailableVersionAsync()
        {
            var changeQueryApi = await _apiFacade.GetApiClassInstance<AvailableChangeVersionsApi>(false, true);
            var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.AvailableChangeVersions);
            long availableVersion = 0;
            var errorMessage = string.Empty;
            ApiResponse<AvailableChangeVersion> currentApiResponse = null;
            try
            {
                currentApiResponse = await changeQueryApi.GetAvailableChangeVersionsWithHttpInfoAsync();
            }
            catch (ApiException exception)
            {
                errorMessage = exception.Message;
                if (exception.ErrorCode.Equals((int) HttpStatusCode.Unauthorized))
                {
                    changeQueryApi = await _apiFacade.GetApiClassInstance<AvailableChangeVersionsApi>(true, true);
                    currentApiResponse = await changeQueryApi.GetAvailableChangeVersionsWithHttpInfoAsync();
                    errorMessage = string.Empty;
                }
            }

            currentApiResponse = await _responseHandleService.Handle(currentApiResponse, responseUri, errorMessage);

            if (currentApiResponse != null)
            {
                availableVersion = currentApiResponse.Data.NewestChangeVersion;
            }

            return availableVersion;
        }

        public async Task<List<ChangeQuery>> ReadCurrentVersionsForResourcesAsync()
        {
            var currentVersionsForResources = await _dataService.ReadAllAsync<ChangeQuery>();
            return currentVersionsForResources.ToList();
        }
    }
}
