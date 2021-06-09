using System.IO;
using System.Threading.Tasks;
using EdFi.Roster.Sdk.Client;

namespace EdFi.Common
{
    public interface IConfigurationService
    {
        Task<Configuration> ApiConfiguration(bool refreshToken = false, bool isChangeQueries = false);
    }
}
