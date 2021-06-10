using System.Collections.Generic;

namespace EdFi.Roster.ChangeQueries.Models
{
    public class ChangeQueryViewModel
    {
        public string ChangeSummaryMessage { get; set; }

        public IEnumerable<DataSyncResponseModel> SyncResponses { get; set; }
    }
}
