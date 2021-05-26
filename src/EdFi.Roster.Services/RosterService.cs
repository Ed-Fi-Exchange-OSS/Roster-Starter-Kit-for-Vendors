using EdFi.Roster.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdFi.Roster.Services
{
    public class RosterService
    {
        private readonly LocalEducationAgencyService _localEducationAgencyService;
        private readonly SchoolService _schoolService;
        private readonly SectionService _sectionService;
        private readonly StaffService _staffService;
        private readonly StudentService _studentService;

        public RosterService(LocalEducationAgencyService localEducationAgencyService
            , SchoolService schoolService
            , SectionService sectionService
            , StaffService staffService
            , StudentService studentService)
        {
            _localEducationAgencyService = localEducationAgencyService;
            _schoolService = schoolService;
            _sectionService = sectionService;
            _staffService = staffService;
            _studentService = studentService;
        }

        public async Task<LocalEducationAgencyRoster> GetRosterAsync()
        {
            var leas = await _localEducationAgencyService.ReadAllAsync();
            var schools = await _schoolService.ReadAllAsync();
            var sections = await _sectionService.ReadAllAsync();
            var staff = await _staffService.ReadAllAsync();
            var students = await _studentService.ReadAllAsync();

            var returnLeaRoster = new LocalEducationAgencyRoster();
            returnLeaRoster.LocalEducationAgency = leas.ToList()[0];

            returnLeaRoster.SchoolRosters = (from s in schools
                                             select new SchoolRoster
                                             {
                                                 School = s,
                                                 SectionsFull = (from sec2 in sections
                                                                 where sec2.Session.SchoolId == s.SchoolId
                                                                 select new SectionFullPeople
                                                                 {
                                                                     AcademicSubjectDescriptor = sec2.AcademicSubjectDescriptor,
                                                                     AvailableCredits = sec2.AvailableCredits,
                                                                     ClassPeriods = sec2.ClassPeriods,
                                                                     EducationalEnvironmentDescriptor = sec2.EducationalEnvironmentDescriptor,
                                                                     Etag = sec2.Etag,
                                                                     Id = sec2.Id,
                                                                     LocalCourseCode = sec2.LocalCourseCode,
                                                                     LocalCourseTitle = sec2.LocalCourseTitle,
                                                                     Location = sec2.Location,
                                                                     SectionIdentifier = sec2.SectionIdentifier,
                                                                     SequenceOfCourse = sec2.SequenceOfCourse,
                                                                     Session = sec2.Session,
                                                                     Staff = (from st in staff
                                                                              join stsec in sec2.Staff
                                                                      on st.StaffUniqueId equals stsec.StaffUniqueId
                                                                              select st).ToList(),
                                                                     Students = (from stu in students
                                                                                 join stusec in sec2.Students
                                                                         on stu.StudentUniqueId equals stusec.StudentUniqueId
                                                                                 select stu).ToList()
                                                                 }).ToList(),
                                                 Terms = (from sec in sections
                                                          where sec.Session.SchoolId == s.SchoolId
                                                          group sec by sec.Session.TermDescriptor into termGroup
                                                          select termGroup).ToList().Select(x =>
                                                          new Term
                                                          {
                                                              TermDescriptor = x.Key,
                                                              Sections = (from y in x
                                                                          select new SectionFullPeople
                                                                          {
                                                                              AcademicSubjectDescriptor = y.AcademicSubjectDescriptor,
                                                                              AvailableCredits = y.AvailableCredits,
                                                                              ClassPeriods = y.ClassPeriods,
                                                                              EducationalEnvironmentDescriptor = y.EducationalEnvironmentDescriptor,
                                                                              Etag = y.Etag,
                                                                              Id = y.Id,
                                                                              LocalCourseCode = y.LocalCourseCode,
                                                                              LocalCourseTitle = y.LocalCourseTitle,
                                                                              Location = y.Location,
                                                                              SectionIdentifier = y.SectionIdentifier,
                                                                              SequenceOfCourse = y.SequenceOfCourse,
                                                                              Session = y.Session,
                                                                              Staff = (from st in staff
                                                                                       join stsec3 in y.Staff
                                                                               on st.StaffUniqueId equals stsec3.StaffUniqueId
                                                                                       select st).ToList(),
                                                                              Students = (from stu in students
                                                                                          join stusec3 in y.Students
                                                                                  on stu.StudentUniqueId equals stusec3.StudentUniqueId
                                                                                          select stu).ToList()
                                                                          }).ToList()
                                                          }).ToList()
                                             }).ToList();

            foreach (var schoolRoster in returnLeaRoster.SchoolRosters)
            {
                var tempStaffIds = (from t in schoolRoster.SectionsFull
                                    from s in t.Staff
                                    select s.StaffUniqueId).Distinct();

                var resultsForSchool = (from tsi in tempStaffIds
                                        join sts in staff
                                        on tsi equals sts.StaffUniqueId
                                        select new StaffSection
                                        {
                                            StaffInformation = sts,
                                            SectionsFull = (from srsf in schoolRoster.SectionsFull
                                                            where srsf.Staff.Select(p => p.StaffUniqueId).Any(px => px == tsi)
                                                            select srsf).ToList()
                                        }).ToList();

                schoolRoster.StaffSections = resultsForSchool;
            }

            var staffSecs = new List<EdFi.Roster.Models.StaffSection>();

            staffSecs = (from staffs1 in staff
                         select new EdFi.Roster.Models.StaffSection
                         {
                             StaffInformation = staffs1
                         }).ToList();

            foreach (var staffSec in staffSecs)
            {
                staffSec.SectionsFull = (from secs9 in sections
                                         where secs9.Staff.Any(p => p.StaffUniqueId == staffSec.StaffInformation.StaffUniqueId)
                                         select new SectionFullPeople
                                         {
                                             AcademicSubjectDescriptor = secs9.AcademicSubjectDescriptor,
                                             AvailableCredits = secs9.AvailableCredits,
                                             ClassPeriods = secs9.ClassPeriods,
                                             EducationalEnvironmentDescriptor = secs9.EducationalEnvironmentDescriptor,
                                             Etag = secs9.Etag,
                                             Id = secs9.Id,
                                             LocalCourseCode = secs9.LocalCourseCode,
                                             LocalCourseTitle = secs9.LocalCourseTitle,
                                             Location = secs9.Location,
                                             SectionIdentifier = secs9.SectionIdentifier,
                                             SequenceOfCourse = secs9.SequenceOfCourse,
                                             Session = secs9.Session,
                                             Staff = (from st in staff
                                                      join stsec10 in secs9.Staff
                                              on st.StaffUniqueId equals stsec10.StaffUniqueId
                                                      select st).ToList(),
                                             Students = (from stu in students
                                                         join stusec11 in secs9.Students
                                                 on stu.StudentUniqueId equals stusec11.StudentUniqueId
                                                         select stu).ToList()
                                         }).ToList();
            }

            return returnLeaRoster;
        }

    }
}
