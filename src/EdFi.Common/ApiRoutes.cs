namespace EdFi.Common
{
    public static class ApiRoutes
    {
        public static string EnrollmentCompositesBase => "composites/v1";
        public static string ResourcesBase => "data/v3";
        public static string ChangeQueriesBase => "ChangeQueries/v1";

        public static string LocalEducationAgenciesComposite => "ed-fi/enrollment/LocalEducationAgencies";
        public static string SchoolsComposite => "ed-fi/enrollment/Schools";
        public static string StaffsComposite => "ed-fi/enrollment/Staffs";
        public static string StudentsComposite => "ed-fi/enrollment/Students";
        public static string SectionsComposite => "ed-fi/enrollment/Sections";

        public static string LocalEducationAgenciesResource => "ed-fi/localEducationAgencies";
        public static string SchoolsResource => "ed-fi/schools";
        public static string StaffsResource => "ed-fi/staffs";
        public static string StudentsResource => "ed-fi/students";
        public static string SectionsResource => "ed-fi/sections/";

        public static string AvailableChangeVersions => "availableChangeVersions";
    }
}
