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
    public class SectionService
    {
        private readonly IDataService _dataService;
        private readonly IResponseHandleService _responseHandleService;
        private readonly IApiFacade _apiFacade;

        public SectionService(IDataService dataService
            , IResponseHandleService responseHandleService
            , IApiFacade apiFacade)
        {
            _dataService = dataService;
            _responseHandleService = responseHandleService;
            _apiFacade = apiFacade;
        }

        public async Task<IEnumerable<EdFiSection>> ReadAllAsync()
        {
            var sections = await _dataService.ReadAllAsync<RosterSectionResource>();
            return sections.Select(section => JsonConvert.DeserializeObject<EdFiSection>(section.Content)).ToList();
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncSections(long minVersion, long maxVersion)
        {
            var sectionsApi = await _apiFacade.GetApiClassInstance<SectionsApi>();
            var limit = 100;
            var offset = 0;
            var response = new ExtendedInfoResponse<List<EdFiSection>>();
            var currResponseRecordCount = 0;

            do
            {
                var errorMessage = string.Empty;
                var responseUri = _apiFacade.BuildResponseUri(ApiRoutes.SectionsResource, offset, limit);
                ApiResponse<List<EdFiSection>> currentApiResponse = null;
                try
                {
                    currentApiResponse = await sectionsApi.GetSectionsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                }
                catch (ApiException exception)
                {
                    errorMessage = exception.Message;
                    if (exception.ErrorCode.Equals((int)HttpStatusCode.Unauthorized))
                    {
                        sectionsApi = await _apiFacade.GetApiClassInstance<SectionsApi>(true);
                        currentApiResponse = await sectionsApi.GetSectionsWithHttpInfoAsync(offset, limit, (int?)minVersion, (int?)maxVersion);
                        errorMessage = string.Empty;
                    }
                }

                if (currentApiResponse == null) continue;
                currResponseRecordCount = currentApiResponse.Data.Count;
                offset += limit;
                response = await _responseHandleService.Handle(currentApiResponse, response, responseUri, errorMessage);

            } while (currResponseRecordCount >= limit);

            // Sync retrieved records to local db
            var sections = response.FullDataSet.Select(section =>
                new RosterSectionResource { Content = JsonConvert.SerializeObject(section), ResourceId = section.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(sections);

            // TODO: Call Deletes endpoint to get deleted records and update local db accordingly

            // TODO: Update Change query table to reflect latest available version for Sections

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Sections,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = 0
            };
        }
    }
}
