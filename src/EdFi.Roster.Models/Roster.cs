using EdFi.Roster.Sdk.Models.EnrollmentComposites;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdFi.Roster.Models
{
    public class LocalEducationAgencyRoster
    {
        public LocalEducationAgency LocalEducationAgency { get; set; }
        public List<SchoolRoster> SchoolRosters { get; set; }
    }

    public class SchoolRoster
    {
        public School School { get; set; }
        public List<Term> Terms { get; set; }
        public List<SectionFullPeople> SectionsFull { get; set; }
        public List<StaffSection> StaffSections { get; set; }
    }

    public class StaffSection
    {
        public Staff StaffInformation { get; set; }
        public List<SectionFullPeople> SectionsFull { get; set; }
    }
    public class Term
    {
        public string TermDescriptor { get; set; }
        public List<SectionFullPeople> Sections { get; set; }
    }
    public class SectionFullPeople : Section
    {
        public new IList<Student> Students { get; set; }
        public new IList<Staff> Staff { get; set; }
    }
}
