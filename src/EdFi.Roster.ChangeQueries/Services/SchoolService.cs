using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Client;
using EdFi.Roster.Sdk.Models.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class SchoolService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public SchoolService(IDataService dataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _dataService = dataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task<IEnumerable<EdFiSchool>> ReadAllAsync()
        {
            var schools = await _dataService.ReadAllAsync<RosterSchoolResource>();
            return schools.Select(school => JsonConvert.DeserializeObject<EdFiSchool>(school.Content)).ToList();
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncSchools(long minVersion, long maxVersion)
        {
            var schoolsApi = await _apiFacade.GetApiClassInstance<SchoolsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiSchool>>();
            var currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.SchoolsResource, offset, limit);
                ApiResponse<List<EdFiSchool>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await schoolsApi.GetSchoolsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        schoolsApi = await _apiFacade.GetApiClassInstance<SchoolsApi>(true);
                        currentApiResponse = await schoolsApi.GetSchoolsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            // Sync retrieved records to local db
            var schools = response.FullDataSet.Select(school =>
                new RosterSchoolResource { Content = JsonConvert.SerializeObject(school), ResourceId = school.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(schools);

            // TODO: Call Deletes endpoint to get deleted records and update local db accordingly

            // TODO: Update Change query table to reflect latest available version for Schools

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Schools,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = 0
            };
        }
    }
}
