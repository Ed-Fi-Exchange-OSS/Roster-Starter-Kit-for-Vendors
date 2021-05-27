using System;
using System.Collections.Generic;

namespace EdFi.Roster.Models
{
    public class ExtendedInfoResponse<T> where T : new()
    {
        public ExtendedInfoResponse()
        {
            this.FullDataSet = new T();
            GeneralInfo =  new GeneralInfoResponse();
        }
       
        public GeneralInfoResponse GeneralInfo { get; set; }

        public T FullDataSet { get; set; }
       
        public bool IsExtendedInfoAvailable { get; set; }
    }
    public class ExtendedInfoResponsePage
    {
        public int RecordsCount { get; set; }
        public Uri ResponseUri { get; set; }
    }

    public class GeneralInfoResponse
    {
        public GeneralInfoResponse()
        {
            this.Pages = new List<ExtendedInfoResponsePage>();
        }

        public int TotalRecords { get; set; }

        public List<ExtendedInfoResponsePage> Pages { get; set; }

        public string ResponseData { get; set; }
    }
}
