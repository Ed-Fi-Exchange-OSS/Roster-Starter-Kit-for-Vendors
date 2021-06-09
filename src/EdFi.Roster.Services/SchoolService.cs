using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.Sdk.Api.EnrollmentComposites;
using EdFi.Roster.Sdk.Client;
using Newtonsoft.Json;

namespace EdFi.Roster.Services
{
    public class SchoolService
    {
        private readonly IRosterDataService _rosterDataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public SchoolService(IRosterDataService rosterDataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _rosterDataService = rosterDataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task Save(List<School> schools)
        {
            var schoolList = schools.Select(school =>
                new RosterSchoolComposite {Content = JsonConvert.SerializeObject(school), ResourceId = school.Id}).ToList();

            await _rosterDataService.SaveAsync(schoolList);
        }

        public async Task<IEnumerable<School>> ReadAllAsync()
        {
            var schools = await _rosterDataService.ReadAllAsync<RosterSchoolComposite>();
            return schools.Select(school => JsonConvert.DeserializeObject<School>(school.Content)).ToList();
        }

        public async Task<ExtendedInfoResponse<List<School>>> GetAllSchoolsWithExtendedInfoAsync()
        {
            var api = await _apiFacade.GetApiClassInstance<SchoolsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<School>>();
            int currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.SchoolsComposite, offset, limit);
                ApiResponse<List<School>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await api.GetSchoolsWithHttpInfoAsync(offset, limit); 
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        api = await _apiFacade.GetApiClassInstance<SchoolsApi>(true);
                        currentApiResponse = await api.GetSchoolsWithHttpInfoAsync(offset, limit);
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
