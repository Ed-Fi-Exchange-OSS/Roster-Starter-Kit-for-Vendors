using EdFi.Roster.Models;

namespace EdFi.Roster.ChangeQueries.Models
{
    public class DataSyncResponseModel
    {
        public DataSyncResponseModel()
        {
            ChangeDetails = new GeneralInfoResponse();
            DeletionDetails = new GeneralInfoResponse();
        }

        public string ResourceName { get; set; }

        public int UpdatedRecordsCount { get; set; }

        public int AddedRecordsCount { get; set; }

        public int DeletedRecordsCount { get; set; }

        public GeneralInfoResponse ChangeDetails { get; set; }
        public GeneralInfoResponse DeletionDetails { get; set; }
    }
}
