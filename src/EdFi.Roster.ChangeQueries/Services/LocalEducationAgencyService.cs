using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class LocalEducationAgencyService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public LocalEducationAgencyService(IDataService dataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _dataService = dataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task Save(List<EdFiLocalEducationAgency> localEducationAgencies)
        {
            var leas = localEducationAgencies.Select(lea =>
                new RosterLocalEducationAgencyResource {Content = JsonConvert.SerializeObject(lea), ResourceId = lea.Id}).ToList();

             await _dataService.SaveAsync(leas);
        }

        public async Task<IEnumerable<EdFiLocalEducationAgency>> ReadAllAsync()
        {
            var leas = await _dataService.ReadAllAsync<RosterLocalEducationAgencyResource>();
            return leas.Select(lea => JsonConvert.DeserializeObject<EdFiLocalEducationAgency>(lea.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<EdFiLocalEducationAgency>>> GetAllLocalEducationAgenciesWithExtendedInfoAsync()
        {
            var leaApi = await _apiFacade.GetApiClassInstance<LocalEducationAgenciesApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiLocalEducationAgency>>();
            int currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.LocalEducationAgenciesResource, offset, limit);
                ApiResponse<List<EdFiLocalEducationAgency>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await leaApi.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        leaApi = await _apiFacade.GetApiClassInstance<LocalEducationAgenciesApi>(true);
                        currentApiResponse = await leaApi.GetLocalEducationAgenciesWithHttpInfoAsync(offset, limit);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            response.GeneralInfo.TotalRecords = response.FullDataSet.Count;
            response.GeneralInfo.ResponseData = JsonConvert.SerializeObject(response.FullDataSet, Formatting.Indented);

            return response;
        }
    }
}

