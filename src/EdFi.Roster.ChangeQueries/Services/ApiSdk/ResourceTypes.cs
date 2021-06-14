namespace EdFi.Roster.ChangeQueries.Services.ApiSdk
{
    public static class ResourceTypes
    {
        public const string LocalEducationAgencies = "LocalEducationAgencies";
        public const string Schools = "Schools";
        public const string Staff = "Staff";
        public const string Students = "Students";
        public const string Sections = "Sections";

        public static int ResourceTypeCount()
        {
            return typeof(ResourceTypes).GetFields().Length;
        }
    }
}
