namespace EdFi.Roster.ChangeQueries.Models
{
    public class DataSyncResponseModel
    {
        public string ResourceName { get; set; }

        public int UpdatedRecordsCount { get; set; }

        public int AddedRecordsCount { get; set; }

        public int DeletedRecordsCount { get; set; }
    }
}
