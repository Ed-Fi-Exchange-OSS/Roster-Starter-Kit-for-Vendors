using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Services.ApiSdk;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public class LocalEducationAgencyService
    {
        private readonly IRosterDataService _rosterDataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public LocalEducationAgencyService(IRosterDataService rosterDataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _rosterDataService = rosterDataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task Save(List<LocalEducationAgency> localEducationAgencies)
        {
            var leas = localEducationAgencies.Select(lea =>
                new RosterLocalEducationAgencyComposite {Content = JsonConvert.SerializeObject(lea), ResourceId = lea.Id}).ToList();

             await _rosterDataService.SaveAsync(leas);
        }

        public async Task<IEnumerable<LocalEducationAgency>> ReadAllAsync()
        {
            var leas = await _rosterDataService.ReadAllAsync<RosterLocalEducationAgencyComposite>();
            return leas.Select(lea => JsonConvert.DeserializeObject<LocalEducationAgency>(lea.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<LocalEducationAgency>>> GetAllLocalEducationAgenciesWithExtendedInfoAsync()
        {
            var leaApi = await _apiFacade.GetApiClassInstance<LocalEducationAgenciesApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<LocalEducationAgency>>();
            int currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.LocalEducationAgencies, offset, limit);
                ApiResponse<List<LocalEducationAgency>> currentApiResponse = null;
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

