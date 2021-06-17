using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common;
using EdFi.Roster.ChangeQueries.Models;
using EdFi.Roster.ChangeQueries.Services.ApiSdk;
using EdFi.Roster.Models;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Models.Resources;
using Newtonsoft.Json;

namespace EdFi.Roster.ChangeQueries.Services
{
    public class StudentService
    {
        private readonly IDataService _dataService;
        private readonly ApiService _apiService;
        private readonly ChangeQueryService _changeQueryService;

        public StudentService(IDataService dataService
            , ApiService apiService
            , ChangeQueryService changeQueryService)
        {
            _dataService = dataService;
            _apiService = apiService;
            _changeQueryService = changeQueryService;
        }

        public async Task<DataSyncResponseModel> RetrieveAndSyncResources(long minVersion, long maxVersion)
        {

            var queryParams = new Dictionary<string, string> { { "minChangeVersion", minVersion.ToString() },
                { "maxChangeVersion", maxVersion.ToString() } };

            var response =
                await _apiService.GetAllResources<StudentsApi, EdFiStudent>(
                    $"{ApiRoutes.StudentsResource}", queryParams,
                    async (api, offset, limit) =>
                        await api.GetStudentsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync retrieved records to local db
            var students = response.FullDataSet.Select(student =>
                new RosterStudentResource
                { Content = JsonConvert.SerializeObject(student), ResourceId = student.Id }).ToList();
            var addedRecords = await _dataService.AddOrUpdateAllAsync(students);

            var deletesResponse =
                await _apiService.GetAllResources<StudentsApi, DeletedResource>(
                    $"{ApiRoutes.StudentsResource}/deletes", queryParams,
                    async (api, offset, limit) =>
                        await api.DeletesStudentsWithHttpInfoAsync(
                            offset, limit, (int?)minVersion, (int?)maxVersion));

            // Sync deleted records to local db
            var deletedStudentsCount = 0;
            if (deletesResponse.FullDataSet.Any())
            {
                var deletedStudents = deletesResponse.FullDataSet.Select(student => student.Id).ToList();
                await _dataService.DeleteAllAsync<RosterStudentResource>(deletedStudents);
                deletedStudentsCount = deletedStudents.Count;
            }

            // Save latest change version 
            await _changeQueryService.Save(maxVersion, ResourceTypes.Students);

            return new DataSyncResponseModel
            {
                ResourceName = ResourceTypes.Students,
                AddedRecordsCount = addedRecords,
                UpdatedRecordsCount = response.FullDataSet.Count - addedRecords,
                DeletedRecordsCount = deletedStudentsCount
            };
        }
    }
}
