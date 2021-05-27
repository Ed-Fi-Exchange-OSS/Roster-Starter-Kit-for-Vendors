using System;

namespace EdFi.Roster.Models
{
    public class ApiLogEntry
    {
        public int Id { get; set; }
        public DateTime LogDateTime { get; set; }
        public string StatusCode { get; set; }
        public string Method { get; set; }
        public string Uri { get; set; }
        public string Content { get; set; }
        public string ErrorMessage { get; set; }
    }
}
