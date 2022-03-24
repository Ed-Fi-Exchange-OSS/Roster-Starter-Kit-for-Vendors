using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Roster.Sdk.Api.Descriptors;
using EdFi.Roster.Sdk.Api.Resources;
using EdFi.Roster.Sdk.Models.Resources;
using EdFi.Roster.Sdk.Client;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace get_students_by_grade
{
    class Program
    {
        const int PAGE_SIZE = 500;

        static void Main(string[] args)
        {
            GetStudents(args).Wait();
        }

        static async Task GetStudents(string[] args) {
            Console.WriteLine("Getting all students for a school by grade level...");

            if (args.Length < 5)
            {
                Console.WriteLine(string.Join(",", args));
                Console.Error.WriteLine("Must provide following arguments: base url, key, secret, school ID, grade level");
            }


            // This is quick-and-dirty code without appropriate exception handling. Please modify as needed to suite your needs.

            var baseUrl = args[0]; // ex: https://api.ed-fi.org/v5.2/api
            var dataUrl = $"{baseUrl}/data/v3";
            var key = args[1];
            var secret = args[2];
            var schoolId = int.Parse(args[3]);
            var gradeLevel = args[4];

            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            var tokenRetriever = new TokenRetriever(baseUrl, key, secret);

            // Tokens will need to be refreshed when they expire - not handled in this example
            var configuration = new Configuration() { AccessToken = tokenRetriever.ObtainNewBearerToken(), BasePath = dataUrl };


            // Assuming there are < 500 grade descriptors, therefore not bothering with paging
            Console.WriteLine("Get Grade Level Descriptors...");
            var descriptors = await new GradeLevelDescriptorsApi(configuration).GetGradeLevelDescriptorsAsync(limit: PAGE_SIZE);

            // Get the specific grade level set in the arguments
            var descriptor = descriptors.FirstOrDefault(x => x.CodeValue.ToLower() == gradeLevel.ToLower());
            if (descriptor.CodeValue != gradeLevel)
            {
                Console.Error.WriteLine($"There is no grade level descriptor with short description {gradeLevel}");
                System.Environment.Exit(1);
            }

            var descriptorUri = $"{descriptor.Namespace}#{descriptor.CodeValue}";

            var schoolAssociationsApi = new StudentSchoolAssociationsApi(configuration);
            var page = 0;

            // Retrieve all StudentSchoolAssociations for the given grade level and School ID
            Console.WriteLine("Get first page of StudentSchoolAssociations...");
            var schoolAssociationsResponse = await schoolAssociationsApi.GetStudentSchoolAssociationsAsync(offset: page* PAGE_SIZE, limit: PAGE_SIZE, entryGradeLevelDescriptor: descriptorUri, schoolId: schoolId);
            var schoolAssociation = new List<EdFiStudentSchoolAssociation>(schoolAssociationsResponse);
            while (schoolAssociationsResponse.Count() == PAGE_SIZE)
            {
                schoolAssociation.AddRange(schoolAssociationsResponse);
                ++page;
                Console.WriteLine($"Getting page {page} of StudentSchoolAssociations...");
                schoolAssociationsResponse = await schoolAssociationsApi.GetStudentSchoolAssociationsAsync(offset: page* PAGE_SIZE, limit: PAGE_SIZE, entryGradeLevelDescriptor: descriptorUri);
            }

            // Using the StudentSchoolAssociations, now retrieve each student
            var studentsApi = new StudentsApi(configuration);

            Console.WriteLine("Getting Students...");
            const string outputPath = "json-output";
            Directory.CreateDirectory(outputPath);

            foreach (var association in schoolAssociation) {
                var id = association.StudentReference.StudentUniqueId;
                var student = await studentsApi.GetStudentsAsync(studentUniqueId: id);

                // What do you want to do with this student? Let's write out to JSON file just for fun.
                var serialized = JsonSerializer.Serialize(student);
                File.WriteAllText(Path.Join("json-output", $"{id}.json"), serialized);
            }

            Console.WriteLine("Done.");
        }
    }
}
