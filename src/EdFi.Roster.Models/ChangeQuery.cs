using System;

namespace EdFi.Roster.Models
{
    public class ChangeQuery
    {
        public int Id { get; set; }
        public string ResourceType { get; set; }
        public long ChangeVersion { get; set; }
    }
}
