using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Roster.Models;

namespace EdFi.Roster.Services
{
    public class ApiLogService
    {
        private readonly IRosterDataService _dataService;

        public ApiLogService(IRosterDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task WriteLog(ApiLogEntry logEntry)
        {
            await _dataService.SaveAsync(new List<ApiLogEntry>
            {
                logEntry
            }, true);
        }

        public void ClearLogs()
        {
            _dataService.ClearRecords<ApiLogEntry>();
        }

        public async Task<List<ApiLogEntry>> ReadAllLogsAsync()
        {
            var logEntries = await _dataService.ReadAllAsync<ApiLogEntry>();
            return logEntries.ToList();
        }
    }
}
